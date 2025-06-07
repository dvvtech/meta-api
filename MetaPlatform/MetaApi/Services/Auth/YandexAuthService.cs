using MetaApi.Configuration.Auth;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MetaApi.Services.Auth
{
    public partial class YandexAuthService
    {
        private readonly IMemoryCache _cache;
        private readonly YandexAuthConfig _authConfig;
        private readonly IJwtProvider _jwtProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<YandexAuthService> _logger;

        public YandexAuthService(IMemoryCache cache,
                                 IOptions<YandexAuthConfig> authConfig,
                                 IAccountRepository accountRepository,
                                 IJwtProvider jwtProvider,
                                 HttpClient httpClient,
                                 ILogger<YandexAuthService> logger)
        {
            _cache = cache;
            _authConfig = authConfig.Value;
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _httpClient = httpClient;
            _logger = logger;
        }                
    }    
}
