using MetaApi.Models.VirtualFit;
using MetaApi.Utilities;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Services.Interfaces;
using MetaApi.Core.Domain.FittingHistory;

namespace MetaApi.Services
{    
    public class ReplicateClientService : IReplicateClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReplicateClientService> _logger;
        private const string PredictionModelVersion = "0513734a452173b8173e907e3a59d19a36266e55b48528559432bd21c7d7e985";
        private const int MaxRetries = 15;
        private const int RetryDelayMilliseconds = 1500;

        public ReplicateClientService(HttpClient httpClient,
                                      ILogger<ReplicateClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<Result<string>> ProcessPredictionAsync(FittingData data)
        {            
            try
            {
                var predictionRequest = CreatePredictionRequest(data);
                var jsonContent = SerializeToJson(predictionRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("predictions", content);
                if (!response.IsSuccessStatusCode)
                {
                    string httpStatusCode = ((int)response.StatusCode).ToString();
                    return Result<string>.Failure(PredictionErrors.PredictionRequestFail(httpStatusCode));
                }

                var predictionId = await ExtractPredictionId(response);
                return await CheckPredictionStatus(_httpClient, predictionId);                
            }
            catch (Exception ex)
            {                
                return Result<string>.Failure(PredictionErrors.PredictionException(ex.ToString()));
            }
        }

        private PredictionRequest CreatePredictionRequest(FittingData data)
        {            
            return new PredictionRequest
            {
                Version = PredictionModelVersion,
                Input = new InputData
                {
                    Crop = false,
                    Seed = 42,
                    Steps = 30,
                    Category = data.Category,
                    ForceDc = false,
                    GarmImg = ImageUrlHelper.GetUrl(data.GarmImg),
                    HumanImg = ImageUrlHelper.GetUrl(data.HumanImg),
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

        private async Task<string> ExtractPredictionId(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            return document.RootElement.GetProperty("id").GetString();
        }

        private async Task<Result<string>> CheckPredictionStatus(HttpClient httpClient, string predictionId)
        {                        
            var checkUrl = $"predictions/{predictionId}";
            int retryCount = 0;

            while (retryCount < MaxRetries)
            {
                await Task.Delay(RetryDelayMilliseconds);
                var checkResponse = await httpClient.GetAsync(checkUrl);

                if (!checkResponse.IsSuccessStatusCode)
                {
                    string httpStatusCode = ((int)checkResponse.StatusCode).ToString();
                    return Result<string>.Failure(PredictionErrors.PredictionStatusFail(httpStatusCode));
                }

                var checkContent = await checkResponse.Content.ReadAsStringAsync();
                using var checkDocument = JsonDocument.Parse(checkContent);
                var status = checkDocument.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    var outputUrl = checkDocument.RootElement.GetProperty("output").GetString();
                    if (string.IsNullOrEmpty(outputUrl))
                    {
                        return Result<string>.Failure(PredictionErrors.PredictionImageEmpty());                                                
                    }

                    return Result<string>.Success(outputUrl);                    
                }
                else if (status == "failed")
                {
                    return Result<string>.Failure(PredictionErrors.PredictionProcessFail(checkContent));
                }

                retryCount++;
            }

            return Result<string>.Failure(PredictionErrors.PredictionTimeout());
        }        
    }
}
