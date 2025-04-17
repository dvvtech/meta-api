using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using MetaApi.Utilities;
using System.Text.Json;
using System.Text;
using MetaApi.Consts;
using Microsoft.EntityFrameworkCore;
using MetaApi.Constants;
using System.Text.Json.Serialization;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {        
        /// <summary>
        /// Попытка примерки одежды.
        /// </summary>
        public async Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request, string host, int userId)
        {            
            var httpClient = _httpClientFactory.CreateClient(ApiNames.REPLICATE_API_CLIENT_NAME);

            var predictionRequest = CreatePredictionRequest(request);
            var jsonContent = SerializeToJson(predictionRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("predictions", content);            
            if (!response.IsSuccessStatusCode)
            {
                return HandleErrorResponse(response);
            }

            var predictionId = await ExtractPredictionId(response);
            var predictionResult = await CheckPredictionStatus(httpClient, predictionId);

            if (predictionResult.status == "succeeded")
            {
                return await ProcessSuccessfulPrediction(request, host, userId, predictionResult.outputUrl);
            }

            return Result<FittingResultResponse>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));            
        }

        private PredictionRequest CreatePredictionRequest(FittingRequest request)
        {
            const string predictionModelVersion = "0513734a452173b8173e907e3a59d19a36266e55b48528559432bd21c7d7e985";

            return new PredictionRequest
            {
                Version = predictionModelVersion,
                Input = new InputData
                {
                    Crop = false,
                    Seed = 42,
                    Steps = 30,
                    Category = request.Category,
                    ForceDc = false,
                    GarmImg = GetUrl(request.GarmImg),
                    HumanImg = GetUrl(request.HumanImg),
                    MaskOnly = false,
                    GarmentDes = ""
                }
            };
        }

        private string SerializeToJson(PredictionRequest requestData)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(requestData, options);
        }

        private Result<FittingResultResponse> HandleErrorResponse(HttpResponseMessage response)
        {
            var errorContent = response.Content.ReadAsStringAsync().Result;
            string errorInfo = $"API request failed with status {response.StatusCode}: {errorContent}";
            return Result<FittingResultResponse>.Failure(VirtualFitError.ThirdPartyServiceError(errorInfo, response.StatusCode));
        }

        private async Task<string> ExtractPredictionId(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            return document.RootElement.GetProperty("id").GetString();
        }

        private async Task<(string status, string outputUrl)> CheckPredictionStatus(HttpClient httpClient, string predictionId)
        {
            const int MaxRetries = 15;
            //Задержка между запросами в миллисекундах (2 секунды)
            const int RetryDelayMilliseconds = 2000;

            var checkUrl = $"predictions/{predictionId}";
            int retryCount = 0;

            while (retryCount < MaxRetries)
            {
                await Task.Delay(RetryDelayMilliseconds);
                var checkResponse = await httpClient.GetAsync(checkUrl);

                if (!checkResponse.IsSuccessStatusCode)
                {
                    return ("failed", "");
                }

                var checkContent = await checkResponse.Content.ReadAsStringAsync();
                using var checkDocument = JsonDocument.Parse(checkContent);
                var status = checkDocument.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    var outputUrl = checkDocument.RootElement.GetProperty("output").GetString();
                    if (string.IsNullOrEmpty(outputUrl))
                    {
                        throw new InvalidOperationException("Output URL is missing or empty.");
                    }

                    return (status, outputUrl);
                }
                else if (status == "failed")
                {
                    return (status, "");
                }

                retryCount++;
            }

            return ("failed", "");
        }

        private FittingResultEntity CreateFittingResultEntity(FittingRequest request, string urlResult, int userId)
        {
            return new FittingResultEntity
            {
                GarmentImgUrl = GetUrl(request.GarmImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                       .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                HumanImgUrl = GetUrl(request.HumanImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                      .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                ResultImgUrl = urlResult.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                AccountId = userId,
                CreatedUtcDate = DateTime.UtcNow
            };
        }

        private async Task<Result<FittingResultResponse>> ProcessSuccessfulPrediction(FittingRequest request,
                                                                                      string host,
                                                                                      int userId,
                                                                                      string outputUrl)
        {
            var urlResult = await _fileService.UploadResultFileAsync(outputUrl, host, request.HumanImg);

            var fittingResult = CreateFittingResultEntity(request, urlResult, userId);
            _metaDbContext.FittingResult.Add(fittingResult);
            await _metaDbContext.SaveChangesAsync();

            return Result<FittingResultResponse>.Success(new FittingResultResponse
            {
                Url = urlResult ?? string.Empty,
                RemainingUsage = await GetRemainingUsage(userId)
            });
        }

        /// <summary>
        /// Кол-во оставшихся попыток
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<int> GetRemainingUsage(int userId)
        {
            var limit = await _metaDbContext.UserTryOnLimits.FirstOrDefaultAsync(l => l.AccountId == userId);
            if (limit != null)
            {
                //-1 так как на текущий момент попытка еще не вычтена, она вычитается в middleware после запроса
                return limit.MaxAttempts - limit.AttemptsUsed - 1;
            }

            return 0;
        }

        private string GetUrl(string url)
        {
            string imgFileName = Path.GetFileNameWithoutExtension(url);
            var underscoreCount = imgFileName.Count(c => c == '_');
            if (underscoreCount == 1)
            {
                //todo если оканчивается на _t то заменить на _v
                if (imgFileName.EndsWith(FittingConstants.THUMBNAIL_SUFFIX_URL, StringComparison.OrdinalIgnoreCase))
                {
                    return url.Replace(FittingConstants.THUMBNAIL_SUFFIX_URL, FittingConstants.FULLSIZE_SUFFIX_URL);
                }

                return url;
            }

            //если фото из внутренней коллекции и для него есть паддинг, то нужно заменить название файла, чтоб оканчивался на _p
            if (imgFileName.EndsWith(FittingConstants.FULLSIZE_SUFFIX_URL, StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.PADDING_SUFFIX_URL);
            }

            //если фото из внутренней коллекции 
            if (imgFileName.EndsWith(FittingConstants.THUMBNAIL_SUFFIX_URL, StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace(FittingConstants.THUMBNAIL_SUFFIX_URL, FittingConstants.PADDING_SUFFIX_URL);
            }

            throw new InvalidOperationException("Invalid image URL format");
        }        
        

        public async Task<Result<FittingResultResponse>> TryOnClothesFakeAsync(FittingRequest request)
        {            
            await Task.Delay(5000);

            return Result<FittingResultResponse>.Success(new FittingResultResponse
            {
                Url = "https://a30944-8332.x.d-f.pw/result/d211d593-59b4-497b-8368-8d13b14f8dc1.jpg",
                RemainingUsage = 3
            });
        }
    }
}
