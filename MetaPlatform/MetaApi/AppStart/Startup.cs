using MetaApi.AppStart.Extensions;
using MetaApi.Configuration;
using MetaApi.Constants;
using MetaApi.Core;
using MetaApi.Core.Configurations;
using MetaApi.Services;
using MetaApi.Services.Auth;
using MetaApi.Services.Cache;
using MetaApi.SqlServer.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace MetaApi.AppStart
{
    public partial class Startup
    {
        private WebApplicationBuilder _builder;
        private JwtConfig _jwtConf;

        public void Initialize(WebApplicationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));

            // Настройка ограничения размера тела запроса (10 МБ)
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB для Kestrel
            });

            InitConfigs();
            _builder.Services.ConfigureRateLimit();            
            ConfigureAuth();
            ConfigureClientAPI();

            builder.Services.AddSwaggerGen();
                        
            _builder.Services.ConfigureCors();
            ConfigureDatabase(builder.Configuration);
            AddServices();

            builder.Services.AddControllers();
        }

        private void InitConfigs()
        {
            _builder.Services.Configure<VirtualFitConfig>(_builder.Configuration.GetSection(VirtualFitConfig.SectionName));
            _builder.Services.Configure<GoogleAuthConfig>(_builder.Configuration.GetSection(GoogleAuthConfig.SectionName));
            _builder.Services.Configure<VkAuthConfig>(_builder.Configuration.GetSection(VkAuthConfig.SectionName));            

            _builder.Services.AddOptions<JwtConfig>()
                .Bind(_builder.Configuration.GetSection(JwtConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            _jwtConf = _builder.Configuration.GetSection(JwtConfig.SectionName).Get<JwtConfig>();
            if (_jwtConf == null)
            {
                throw new InvalidOperationException("JWT configuration section is missing or invalid");
            }            
        }

        private void ConfigureClientAPI()
        {
            _builder.Services.AddHttpClient(ApiNames.REPLICATE_API_CLIENT_NAME, client =>
            {
                var virtualFitConfig = _builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(45); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });            
        }

        private void AddServices()
        {
            //храним временные данные для авторизации vk и кешируем бд в кеше памяти
            _builder.Services.AddMemoryCache();


            _builder.Services.AddScoped<ReplicateClientService>();
            _builder.Services.AddScoped<FittingHistoryCache>();
            _builder.Services.AddScoped<TryOnLimitCache>();            

            _builder.Services.AddAllHealthChecks(_builder.Configuration);

            _builder.Services.AddScoped<TryOnLimitService>();
            _builder.Services.AddScoped<VirtualFitService>();

            _builder.Services.AddScoped<ImageService>();
            _builder.Services.AddScoped<FileService>();

            _builder.Services.AddScoped<AccountRepository>();

            _builder.Services.AddSingleton<JwtProvider>();            
            _builder.Services.AddScoped<AuthService>();
            _builder.Services.AddScoped<VkAuthService>();
            _builder.Services.AddScoped<GoogleAuthService>();                  
            
            _builder.Services.AddSingleton<FileCrcHostedService>();
            _builder.Services.AddHostedService(provider => provider.GetService<FileCrcHostedService>());
        }
    }
}
