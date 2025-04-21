using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MetaApi.HealthChecks
{
    //public class ReplicateApiHealthCheck : IHealthCheck
    //{
    //    private readonly IHttpClientFactory _httpClientFactory;

    //    public ReplicateApiHealthCheck(IHttpClientFactory httpClientFactory)
    //    {
    //        _httpClientFactory = httpClientFactory;
    //    }

    //    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    //    {
    //        var httpClient = _httpClientFactory.CreateClient(ApiNames.REPLICATE_API_CLIENT_NAME);

    //        // free test model
    //        var requestData = new
    //        {
    //            version = "2a190b2da92a22a4a1bc9d74352fcee372418df72628ada3ccb4492fa970eef3", // hello-world
    //            input = new { text = "Hello from C#" }
    //        };

    //        try
    //        {
    //            var response = await httpClient.PostAsJsonAsync("predictions", requestData);
    //            var content = await response.Content.ReadAsStringAsync();
    //            return response.IsSuccessStatusCode ?
    //                HealthCheckResult.Healthy("✅ ReplicateAPI is up and runnung") :
    //                HealthCheckResult.Unhealthy("❌ ReplicateAPI is down");
    //        }
    //        catch (Exception ex)
    //        {
    //            return HealthCheckResult.Unhealthy("❌ ReplicateAPI is down");
    //        }
    //    }
    //}
}
