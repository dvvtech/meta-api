using MetaApi.Configuration.Auth;
using MetaApi.Core.Domain.Account;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace MetaApi.Services.Auth
{
    public partial class YandexAuthService
    {
        private readonly IMemoryCache _cache;
        private readonly YandexAuthConfig _authConfig;
        private readonly IJwtProvider _jwtProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<YandexAuthService> _logger;

        public YandexAuthService(IMemoryCache cache,
                                 IOptions<YandexAuthConfig> authConfig,
                                 IAccountRepository accountRepository,
                                 IJwtProvider jwtProvider,
                                 ILogger<YandexAuthService> logger)
        {
            _cache = cache;
            _authConfig = authConfig.Value;
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }        

        
    }

    // Модели для ответов от Яндекс API
    public class YandexTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public class YandexUserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("real_name")]
        public string RealName { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("default_email")]
        public string DefaultEmail { get; set; }

        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; }
    }
}
