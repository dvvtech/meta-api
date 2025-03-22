using MetaApi.Configuration;
using MetaApi.Constants;
using MetaApi.Services;

namespace MetaApi.AppStart
{
    public partial class Startup
    {
        private WebApplicationBuilder _builder;

        public void Initialize(WebApplicationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.Configure<VirtualFitConfig>(builder.Configuration.GetSection(VirtualFitConfig.SectionName));

            var virtualFitConfig = builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

            builder.Services.AddHttpClient(ApiNames.REPLICATE_API, client =>
            {
                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(45); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            builder.Services.AddSwaggerGen();
            
            ConfigureCors();
            ConfigureDatabase(builder.Configuration);

            //храним временные данные для авторизации vk в кеше памяти
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<VkAuthService>();
            builder.Services.AddScoped<VirtualFitService>();
            builder.Services.AddSingleton<FileCrcHostedService>();
            builder.Services.AddHostedService(provider => provider.GetService<FileCrcHostedService>());

            builder.Services.AddControllers();
        }
    }
}
