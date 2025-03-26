using MetaApi.Configuration;
using MetaApi.Core;
using MetaApi.SqlServer.Repositories;
using Microsoft.Extensions.Options;

namespace MetaApi.Services.Auth
{
    public partial class GoogleAuthService
    {
        private readonly GoogleAuthConfig _authConfig;        
        private readonly JwtProvider _jwtProvider;
        private readonly AccountRepository _accountRepository;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IOptions<GoogleAuthConfig> authConfig,
                                 AccountRepository accountRepository,                                 
                                 JwtProvider jwtProvider,
                                 ILogger<GoogleAuthService> logger)
        {
            _authConfig = authConfig.Value;
            _accountRepository = accountRepository;            
            _jwtProvider = jwtProvider;
            _logger = logger;
        }        
    }

    public class TokenResponse
    {
        public Guid UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public long? ExpiresInSeconds { get; set; }

        public string IdToken { get; set; }

        public DateTime IssuedUtc { get; set; }
    }
}
