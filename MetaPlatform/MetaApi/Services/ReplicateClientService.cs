using MetaApi.Constants;
using MetaApi.Models.VirtualFit;
using MetaApi.Utilities;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using MetaApi.Consts;

namespace MetaApi.Services
{
    public class ReplicateClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReplicateClientService> _logger;
        private const string PredictionModelVersion = "0513734a452173b8173e907e3a59d19a36266e55b48528559432bd21c7d7e985";
        private const int MaxRetries = 15;
        private const int RetryDelayMilliseconds = 2000;

        public ReplicateClientService(IHttpClientFactory httpClientFactory,
                                      ILogger<ReplicateClientService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<(string status, string outputUrl)> ProcessPredictionAsync(FittingRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient(ApiNames.REPLICATE_API_CLIENT_NAME);

            var predictionRequest = CreatePredictionRequest(request);
            var jsonContent = SerializeToJson(predictionRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("predictions", content);
            if (!response.IsSuccessStatusCode)
            {
                return ("failed", "");
            }

            var predictionId = await ExtractPredictionId(response);
            var predictionResult = await CheckPredictionStatus(httpClient, predictionId);

            return predictionResult;
        }

        private PredictionRequest CreatePredictionRequest(FittingRequest request)
        {            
            return new PredictionRequest
            {
                Version = PredictionModelVersion,
                Input = new InputData
                {
                    Crop = false,
                    Seed = 42,
                    Steps = 30,
                    Category = request.Category,
                    ForceDc = false,
                    GarmImg = ImageUrlHelper.GetUrl(request.GarmImg),
                    HumanImg = ImageUrlHelper.GetUrl(request.HumanImg),
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

        private async Task<(string status, string outputUrl)> CheckPredictionStatus(HttpClient httpClient, string predictionId)
        {                        
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
    }
}
