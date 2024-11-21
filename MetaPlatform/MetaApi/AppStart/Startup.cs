using MetaApi.Config;
using MetaApi.Constants;

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
                client.Timeout = TimeSpan.FromSeconds(60); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            builder.Services.AddSwaggerGen();

            ConfigureCors();
            
            builder.Services.AddControllers();
        }
    }
}
