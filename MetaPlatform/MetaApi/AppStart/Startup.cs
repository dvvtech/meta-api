using MetaApi.Config;
using MetaApi.Constants;

namespace MetaApi.AppStart
{
    public class Startup
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            builder.Services.Configure<VirtualFitConfig>(builder.Configuration.GetSection(VirtualFitConfig.SectionName));

            var virtualFitConfig = builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

            builder.Services.AddHttpClient(ApiNames.REPLICATE_API, client =>
            {
                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(60); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", virtualFitConfig.ApiToken);
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()  // Разрешить любой источник
                          .AllowAnyMethod()  // Разрешить любые HTTP-методы (GET, POST, PUT и т. д.)
                          .AllowAnyHeader(); // Разрешить любые заголовки
                });
            });

            builder.Services.AddControllers();
        }
    }
}
