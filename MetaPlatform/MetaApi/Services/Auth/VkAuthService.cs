using MetaApi.Configuration.Auth;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MetaApi.Services
{
    public partial class VkAuthService
    {        
        private readonly IMemoryCache _cache;        
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<VkAuthService> _logger;
        private readonly VkAuthConfig _authConfig;
        private readonly HttpClient _httpClient;
        private readonly IAccountRepository _accountRepository;

        public VkAuthService(IMemoryCache cache,
                             ILogger<VkAuthService> logger,                             
                             IAccountRepository accountRepository,
                             IJwtProvider jwtProvider,
                             HttpClient httpClient,
                             IOptions<VkAuthConfig> authConfig)
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
