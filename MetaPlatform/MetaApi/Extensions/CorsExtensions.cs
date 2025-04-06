﻿namespace MetaApi.Extensions
{
    public static class CorsExtensions
    {
        private const string AllowAllPolicy = "AllowAll";
        private const string AllowSpecificOriginPolicy = "AllowSpecificOrigin";

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowSpecificOriginPolicy,
                    policy =>
                    {
                        policy.WithOrigins("https://virtual-fit.one")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });

                options.AddPolicy(AllowAllPolicy, policy =>
                {
                    policy.AllowAnyOrigin()  // Разрешить любой источник
                       .AllowAnyMethod()  // Разрешить любые HTTP-методы (GET, POST, PUT и т. д.)
                       .AllowAnyHeader(); // Разрешить любые заголовки
                });

            });
        }

        public static void ApplyCors(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors(AllowAllPolicy);
            }
            else
            {
                app.UseCors(AllowSpecificOriginPolicy);
            }
        }
    }
}
