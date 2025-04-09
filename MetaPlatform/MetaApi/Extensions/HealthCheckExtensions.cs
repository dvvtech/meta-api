using HealthChecks.UI.Client;
using MetaApi.Configuration;
using MetaApi.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace MetaApi.Extensions
{
    public static class HealthCheckExtensions
    {
        public static void AddAllHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck<DatabseHealthCheck>(nameof(DatabseHealthCheck))
                .AddSqlServer(configuration.GetConnectionString(DatabaseConfig.MetaDbMsSqlConnection)); ;
        }

        public static void ApplyAllHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // Используется UI.Client
            });
        }
    }
}
