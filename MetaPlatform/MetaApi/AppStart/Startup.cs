using MetaApi.AppStart.Extensions;
using MetaApi.Configuration;
using MetaApi.Core;
using MetaApi.Core.Configurations;
using MetaApi.Core.Interfaces;
using MetaApi.Core.Time;
using MetaApi.Infrastructure;
using MetaApi.Services;
using MetaApi.Services.Auth;
using MetaApi.Services.Cache;
using MetaApi.Services.Interfaces;
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
            AddInfrastructure();
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

            _builder.Services.AddOptions<SmtpConfig>()
                .Bind(_builder.Configuration.GetSection(SmtpConfig.SectionName))
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
            _builder.Services.AddHttpClient<IReplicateClientService, ReplicateClientService>((serviceProvider, client) =>
            {
                var virtualFitConfig = _builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(45); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });            
        }

        private void AddInfrastructure()
        {
            _builder.Services.AddSingleton<IEmailSender>(provider =>
            {
                var smtpConfig = _builder.Configuration.GetSection(SmtpConfig.SectionName).Get<SmtpConfig>();

                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<EmailSender>>();

                return new EmailSender(
                    smtpConfig.Host,
                    smtpConfig.Port,
                    smtpConfig.Username,
                    smtpConfig.Password,
                    logger
                );
            });
        }

        private void AddServices()
        {
            //храним временные данные для авторизации vk и кешируем бд в кеше памяти
            _builder.Services.AddMemoryCache();

            _builder.Services.AddScoped<IFittingHistoryRepository, FittingHistoryRepository>();
            _builder.Services.Decorate<IFittingHistoryRepository, CachedFittingHistoryRepository>();            

            _builder.Services.AddScoped<ITryOnLimitRepository, TryOnLimitRepository>();
            _builder.Services.Decorate<ITryOnLimitRepository, CachedTryOnLimitRepository>();

            _builder.Services.AddAllHealthChecks(_builder.Configuration);

            _builder.Services.AddScoped<ISystemTime, SystemTime>();
            _builder.Services.AddScoped<ITryOnLimitService, TryOnLimitService>();
            _builder.Services.AddScoped<IVirtualFitService, VirtualFitService>();

            _builder.Services.AddScoped<ImageService>();
            _builder.Services.AddScoped<IFileService, FileService>(serviceProvider =>
            {
                var fileCrcHostedService = serviceProvider.GetRequiredService<FileCrcHostedService>();
                var imageService = serviceProvider.GetRequiredService<ImageService>();
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var logger = serviceProvider.GetRequiredService<ILogger<FileService>>();
                var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

                return new FileService(fileCrcHostedService, imageService, httpClientFactory, logger, webHostEnvironment.WebRootPath);
            });

            _builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            _builder.Services.AddSingleton<JwtProvider>();            
            _builder.Services.AddScoped<AuthService>();
            _builder.Services.AddScoped<VkAuthService>();
            _builder.Services.AddScoped<GoogleAuthService>();                  
            
            _builder.Services.AddSingleton<FileCrcHostedService>();
            _builder.Services.AddHostedService(provider => provider.GetService<FileCrcHostedService>());
        }
    }
}
