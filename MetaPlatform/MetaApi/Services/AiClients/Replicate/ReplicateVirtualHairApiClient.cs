using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace MetaApi.Services.AiClients.Replicate
{
    /// <summary>
    /// Модель https://replicate.com/subhash25rawat/custom-hair
    /// </summary>
    public class ReplicateVirtualHairApiClient : IReplicateVirtualHairApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReplicateVirtualHairApiClient> _logger;
        private const string PredictionModelVersion = "79fcdf78d664c6f7bfd2468fdd3db2ba514bf5d777300f951f5e4b19cdbdbb5f";
        
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
                version = PredictionModelVersion,
                input = new
                {
                    face_image = data.FaceImg,
                    hair_image = data.HairImg,
                    color_image = data.ColorImg
                }
            };

            //Serialize request
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Send request            
            var response = await _httpClient.PostAsync("predictions", content);
            if (!response.IsSuccessStatusCode)
            {
                string httpStatusCode = ((int)response.StatusCode).ToString();
                return Result<string>.Failure(PredictionErrors.PredictionRequestFail(httpStatusCode));
            }

            //Read and parse response
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            if (document.RootElement.TryGetProperty("urls", out var outputProperty))
            {
                var streamUrl = outputProperty.GetProperty("stream").GetString();
                return Result<string>.Success(streamUrl);
            }

            return Result<string>.Failure(PredictionErrors.PredictionImageEmpty());
        }
    }
}
