﻿using MetaApi.Configuration;
using MetaApi.Constants;
using MetaApi.Core;
using MetaApi.Core.Configurations;
using MetaApi.Services;

namespace MetaApi.AppStart
{
    public partial class Startup
    {
        private WebApplicationBuilder _builder;
        private JwtConfig _jwtConf;

        public void Initialize(WebApplicationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));

            InitConfig();
            ConfigureAuth();

            builder.Services.AddHttpClient(ApiNames.REPLICATE_API, client =>
            {
                var virtualFitConfig = builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();

                client.BaseAddress = new Uri("https://api.replicate.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(45); // Таймаут запроса
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {virtualFitConfig.ApiToken}");
                client.DefaultRequestHeaders.Add("Prefer", "wait");
            });

            builder.Services.AddSwaggerGen();
            
            ConfigureCors();
            ConfigureDatabase(builder.Configuration);
            AddServices();

            builder.Services.AddControllers();
        }

        private void InitConfig()
        {
            _builder.Services.Configure<VirtualFitConfig>(_builder.Configuration.GetSection(VirtualFitConfig.SectionName));

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

        private void AddServices()
        {
            //храним временные данные для авторизации vk в кеше памяти
            _builder.Services.AddMemoryCache();

            _builder.Services.AddSingleton<JwtProvider>();
            _builder.Services.AddScoped<VkAuthService>();
            _builder.Services.AddScoped<VirtualFitService>();
            _builder.Services.AddSingleton<FileCrcHostedService>();
            _builder.Services.AddHostedService(provider => provider.GetService<FileCrcHostedService>());
        }
    }
}
