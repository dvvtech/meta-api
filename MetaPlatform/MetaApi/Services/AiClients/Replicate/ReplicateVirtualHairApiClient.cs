using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace MetaApi.Services.AiClients.Replicate
{
    /// <summary>
    /// Модель https://replicate.com/google/nano-banana
    /// </summary>
    public class ReplicateVirtualHairApiClient : IReplicateVirtualHairApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReplicateVirtualHairApiClient> _logger;
        
        public ReplicateVirtualHairApiClient(
            HttpClient httpClient,
            ILogger<ReplicateVirtualHairApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<string>> ProcessPredictionAsync(HairTryOnData data)
        {
            //Create request
            var requestBody = new
            {
                input = new
                {
                    prompt = "try the hairstyle from the second photo on the first one",
                    image_input = new[] { data.FaceImg, data.HairImg }
                }
            };

            //Serialize request
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Send request            
            var response = await _httpClient.PostAsync("models/google/nano-banana/predictions", content);
            if (!response.IsSuccessStatusCode)
            {
                string httpStatusCode = ((int)response.StatusCode).ToString();
                return Result<string>.Failure(PredictionErrors.PredictionRequestFail(httpStatusCode));
            }

            //Read and parse response
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("output", out var outputProperty))
            {
                var resultUrl = outputProperty.GetString();
                if (!string.IsNullOrEmpty(resultUrl))
                {
                    return Result<string>.Success(resultUrl);
                }

                _logger.LogError(responseContent);
            }

            return Result<string>.Failure(PredictionErrors.PredictionImageEmpty());
        }
    }
}
