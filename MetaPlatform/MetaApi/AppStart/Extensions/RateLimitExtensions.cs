using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MetaApi.AppStart.Extensions
{
    public static class RateLimitExtensions
    {
        private const string LimitPolicyName = "fixed";

        public static void ConfigureRateLimit(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(LimitPolicyName, options =>
                {
                    options.PermitLimit = 4;                   // Максимум 4 запроса
                    options.Window = TimeSpan.FromSeconds(10); // За 10 секунд
                    //options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0; // Очередь из 0 дополнительных запросов
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // Устанавливаем код ошибки
            });
        }

        public static void ApplyRateLimit(this WebApplication app)
        {
            app.UseRouting();
            app.UseRateLimiter();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireRateLimiting(LimitPolicyName); 
            });
        }
    }
}
