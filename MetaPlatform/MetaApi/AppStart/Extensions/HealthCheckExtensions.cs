using HealthChecks.UI.Client;
using MetaApi.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace MetaApi.AppStart.Extensions
{
    public static class HealthCheckExtensions
    {
        public static void AddAllHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnectionString = configuration.GetConnectionString(DatabaseConfig.MetaDbMsSqlConnection);

            services.AddHealthChecks()
                //.AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck))
                .AddSqlServer(sqlServerConnectionString);            
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
