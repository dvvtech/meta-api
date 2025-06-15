using MetaApi.Configuration.Auth;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MetaApi.Services.Auth
{
    public partial class GazpromIdAuthService
    {
        private readonly IMemoryCache _cache;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<GazpromIdAuthService> _logger;
        private readonly GazpromIdAuthConfig _authConfig;
        private readonly HttpClient _httpClient;
        private readonly IAccountRepository _accountRepository;

        public GazpromIdAuthService(IMemoryCache cache,
                                    ILogger<GazpromIdAuthService> logger,
                                    IAccountRepository accountRepository,
                                    IJwtProvider jwtProvider,
                                    HttpClient httpClient,
                                    IOptions<GazpromIdAuthConfig> authConfig)
        {
            _cache = cache;
            _logger = logger;
            _jwtProvider = jwtProvider;
            _authConfig = authConfig.Value;
            _httpClient = httpClient;
            _accountRepository = accountRepository;
        }
    }
}
