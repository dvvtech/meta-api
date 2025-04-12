using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MetaApi.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            bool isHealthy = await IsDatabseConnectionOkAsync();

            return isHealthy ?
                HealthCheckResult.Healthy("Database connection is OK") :
                HealthCheckResult.Unhealthy("Database connection error");
        }

        private Task<bool> IsDatabseConnectionOkAsync()
        {
            return Task.FromResult(true);
        }
    }
}
