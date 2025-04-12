using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using MetaApi.Utilities;
using System.Text.Json;
using System.Text;
using MetaApi.Consts;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {        
        /// <summary>
        /// Попытка примерки одежды.
        /// </summary>
        public async Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request, string host, int userId)
        {            
            var httpClient = _httpClientFactory.CreateClient("ReplicateAPI");

            string logRequestBefore = $"{request.GarmImg} {Environment.NewLine} {request.HumanImg}";
            _logger.LogInformation(logRequestBefore);
            string garmImg = GetUrl(request.GarmImg);
            string humanImg = GetUrl(request.HumanImg);
            string logRequestAfter = $"{garmImg} {Environment.NewLine} {humanImg} {Environment.NewLine}";
            _logger.LogInformation(logRequestAfter);            

            // Подготовьте данные для отправки            
            var internalRequestData = new PredictionRequest
            {                
                Version = "0513734a452173b8173e907e3a59d19a36266e55b48528559432bd21c7d7e985",
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

            // Сериализуйте объект данных в JSON для отправки
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            var jsonContent = JsonSerializer.Serialize(internalRequestData, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Отправьте POST запрос к другому сервису
            var response = await httpClient.PostAsync("predictions", content);
            _logger.LogInformation("get response");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<FittingResultResponse>.Failure(VirtualFitError.ThirdPartyServiceError(errorContent, response.StatusCode));
            }
            _logger.LogInformation("get response2");
            // Получаем ID созданного предсказания из первого ответа
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            var predictionId = document.RootElement.GetProperty("id").GetString();
            var status = document.RootElement.GetProperty("status").GetString();

            // Повторяем запрос на получение результата, если статус не "succeeded"            
            var checkUrl = $"predictions/{predictionId}";
            int maxRetries = 15;
            int retryCount = 0;
            int delay = 2000; // Задержка между запросами в миллисекундах (2 секунды)

            while (status != "succeeded" && status != "failed" && retryCount < maxRetries)
            {
                await Task.Delay(delay);
                var checkResponse = await httpClient.GetAsync(checkUrl);

                if (!checkResponse.IsSuccessStatusCode)
                {
                    var errorContent = await checkResponse.Content.ReadAsStringAsync();
                    return Result<FittingResultResponse>.Failure(VirtualFitError.ThirdPartyServiceError(errorContent, checkResponse.StatusCode));
                }

                var checkContent = await checkResponse.Content.ReadAsStringAsync();
                using var checkDocument = JsonDocument.Parse(checkContent);

                // Обновляем статус и проверяем на завершение
                status = checkDocument.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    var outputUrl = checkDocument.RootElement.GetProperty("output").GetString();
                    if (string.IsNullOrEmpty(outputUrl))
                    {
                        throw new InvalidOperationException("Output URL is missing or empty.");
                    }
                                        
                    string urlResult = await _fileService.UploadResultFileAsync(outputUrl, host, request.HumanImg);

                    var fitingResult = new FittingResultEntity
                    {
                        GarmentImgUrl = garmImg.Replace("_p", FittingConstants.THUMBNAIL_SUFFIX_URL).Replace("_v", FittingConstants.THUMBNAIL_SUFFIX_URL),
                        HumanImgUrl = humanImg.Replace("_p", FittingConstants.THUMBNAIL_SUFFIX_URL).Replace("_v", FittingConstants.THUMBNAIL_SUFFIX_URL),
                        ResultImgUrl = urlResult.Replace("_v", FittingConstants.THUMBNAIL_SUFFIX_URL),                        
                        AccountId = userId,
                        CreatedUtcDate = DateTime.UtcNow,
                    };

                    _metaDbContext.FittingResult.Add(fitingResult);                    
                    // Сохранение в базе данных
                    await _metaDbContext.SaveChangesAsync();

                    _logger.LogInformation("get response3");

                    return Result<FittingResultResponse>.Success(new FittingResultResponse
                    {
                        Url = urlResult ?? string.Empty,
                        RemainingUsage = await GetRemainingUsage(userId)
                    });
                }

                retryCount++;
            }

            return Result<FittingResultResponse>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
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
                return limit.MaxAttempts - limit.AttemptsUsed - 1;//-1 так как на текущий момент попытка еще не вычтена
            }

            return 0;
        }

        private string GetUrl(string url)
        {
            string imgFileName = System.IO.Path.GetFileNameWithoutExtension(url);
            var count = imgFileName.Count(s => s == '_');
            if (count == 1)
            {
                //todo если оканчивается на _t то заменить на _v
                if (imgFileName.EndsWith("_t", StringComparison.OrdinalIgnoreCase))
                {
                    return url.Replace("_t", FittingConstants.FULLSIZE_SUFFIX_URL);
                }

                return url;
            }

            /*if (imgFileName.EndsWith("_p", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }*/

            //если фото из внутренней коллекции и для него есть паддинг то нужно заменить название файла чтоб оканчивался на _p
            if (imgFileName.EndsWith("_v", StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace("_v", FittingConstants.PADDING_SUFFIX_URL);
            }

            //если фото из внутренней коллекции 
            if (imgFileName.EndsWith("_t", StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace("_t", FittingConstants.PADDING_SUFFIX_URL);
            }

            throw new Exception("internal error");
        }        

        

        public async Task<Result<FittingResultResponse>> TryOnClothesFakeAsync(FittingRequest request)
        {            
            await Task.Delay(5000);

            // Сохранение в базе данных
            await _metaDbContext.SaveChangesAsync();

            return Result<FittingResultResponse>.Success(new FittingResultResponse
            {
                Url = "https://a30944-8332.x.d-f.pw/result/d211d593-59b4-497b-8368-8d13b14f8dc1.jpg",
                //RemainingUsage = promocode.RemainingUsage
            });
        }
    }
}
