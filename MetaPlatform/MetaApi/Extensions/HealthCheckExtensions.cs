using HealthChecks.UI.Client;
using MetaApi.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace MetaApi.Extensions
{
    public static class HealthCheckExtensions
    {
        public static void ConfigureHealthChech(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabseHealthCheck>(nameof(DatabseHealthCheck));
        }

        public static void ApplyHealthCheck(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // Используется UI.Client
            });
        }
    }
}
