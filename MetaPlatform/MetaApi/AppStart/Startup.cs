using Meta.Infrastructure;
using MetaApi.AppStart.Extensions;
using MetaApi.Configuration;
using MetaApi.Configuration.Auth;
using MetaApi.Core.Configurations;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Interfaces.Services;
using MetaApi.Core.Services;
using MetaApi.Services;
using MetaApi.Services.AiClients;
using MetaApi.Services.AiClients.Base;
using MetaApi.Services.AiClients.Replicate;
using MetaApi.Services.Auth;
using MetaApi.Services.Cache;
using MetaApi.Services.Interfaces;
using MetaApi.SqlServer.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

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
            _builder.Services.Configure<YandexAuthConfig>(_builder.Configuration.GetSection(YandexAuthConfig.SectionName));
            _builder.Services.Configure<MailRuAuthConfig>(_builder.Configuration.GetSection(MailRuAuthConfig.SectionName));
            _builder.Services.Configure<GazpromIdAuthConfig>(_builder.Configuration.GetSection(GazpromIdAuthConfig.SectionName));
            _builder.Services.Configure<AiClientConfig>(_builder.Configuration.GetSection(AiClientConfig.SectionName));
            _builder.Services.Configure<ProxyConfig>(_builder.Configuration.GetSection(ProxyConfig.SectionName));

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

            _builder.Services.Configure<GoogleRecaptchaConfig>(_builder.Configuration.GetSection(GoogleRecaptchaConfig.SectionName));
        }

        private void ConfigureClientAPI()
        {
            _builder.Services.AddHttpClient<IReplicateVirtualFitApiClient, ReplicateVirtualFitApiClient>((serviceProvider, client) =>
            {
                var virtualFitConfig = _builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(45);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            _builder.Services.AddHttpClient<IReplicateVirtualHairApiClient, ReplicateVirtualHairApiClient>((serviceProvider, client) =>
            {
                var virtualFitConfig = _builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(50);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            _builder.Services.AddHttpClient<IAiClient, ChatGptAiClient>((serviceProvider, client) =>
            {
                var aiClientConfig = _builder.Configuration.GetSection(AiClientConfig.SectionName).Get<AiClientConfig>();

                client.BaseAddress = new Uri("https://api.openai.com/v1/chat/completions");
                client.Timeout = TimeSpan.FromSeconds(35); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {aiClientConfig.OpenAiApiKey}");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                // Получаем настройки прокси из конфигурации
                var proxyConfig = _builder.Configuration.GetSection("ProxySettings").Get<ProxyConfig>();

                if (proxyConfig?.Enabled == true && !string.IsNullOrEmpty(proxyConfig.Ip))
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri($"http://{proxyConfig.Ip}:{proxyConfig.Port}"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false
                    };

                    // Если есть логин и пароль
                    if (!string.IsNullOrEmpty(proxyConfig.Login) && !string.IsNullOrEmpty(proxyConfig.Password))
                    {
                        proxy.Credentials = new NetworkCredential(proxyConfig.Login, proxyConfig.Password);
                    }

                    handler.Proxy = proxy;
                    handler.UseProxy = true;
                }

                return handler;
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

            _builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
        }

        private void AddServices()
        {
            //храним временные данные для авторизации vk и кешируем бд в кеше памяти
            _builder.Services.AddMemoryCache();

            _builder.Services.AddScoped<IEmailBodyGenerator, EmailBodyGenerator>();

            _builder.Services.AddScoped<IHairHistoryRepository, HairHistoryRepository>();

            _builder.Services.AddScoped<IFittingHistoryRepository, FittingHistoryRepository>();
            _builder.Services.Decorate<IFittingHistoryRepository, CachedFittingHistoryRepository>();            

            _builder.Services.AddScoped<ITryOnLimitRepository, TryOnLimitRepository>();
            _builder.Services.Decorate<ITryOnLimitRepository, CachedTryOnLimitRepository>();

            _builder.Services.AddAllHealthChecks(_builder.Configuration);

            _builder.Services.AddScoped<ISystemTime, SystemTime>();
            _builder.Services.AddScoped<ITryOnLimitService, TryOnLimitService>();
            _builder.Services.AddScoped<IVirtualFitService, VirtualFitService>();
            _builder.Services.AddScoped<IVirtualHairStyleService, VirtualHairStyleService>();
            _builder.Services.AddScoped<IProfileService, ProfileService>();

            _builder.Services.AddScoped<ImageService>();
            _builder.Services.AddScoped<IFileService, FileService>(serviceProvider =>
            {
                var fileCrcHostedService = serviceProvider.GetRequiredService<ICrcFileProvider>();
                var imageService = serviceProvider.GetRequiredService<ImageService>();
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var logger = serviceProvider.GetRequiredService<ILogger<FileService>>();
                var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

                return new FileService(fileCrcHostedService, imageService, httpClientFactory, logger, webHostEnvironment.WebRootPath);
            });

            _builder.Services.AddScoped<IAccountRepository, AccountRepository>();
                        
            _builder.Services.AddScoped<IAuthService, AuthService>();
            _builder.Services.AddScoped<VkAuthService>();
            _builder.Services.AddScoped<GoogleAuthService>();
            _builder.Services.AddScoped<YandexAuthService>();
            _builder.Services.AddScoped<MailRuAuthService>();
            _builder.Services.AddScoped<GazpromIdAuthService>();            

            _builder.Services.AddSingleton<ICrcFileProvider, FileCrcHostedService>();
            _builder.Services.AddHostedService(provider => provider.GetService<ICrcFileProvider>() as FileCrcHostedService);            
        }
    }
}
