using MetaApi.Configuration;
using MetaApi.Core;
using MetaApi.SqlServer.Context;
using Microsoft.Extensions.Options;

namespace MetaApi.Services.Auth
{
    public partial class GoogleAuthService
    {
        private readonly GoogleAuthConfig _authConfig;
        private readonly MetaDbContext _metaDbContext;
        private readonly JwtProvider _jwtProvider;

        public GoogleAuthService(IOptions<GoogleAuthConfig> authConfig,
                                 MetaDbContext metaDbContext,
                                 JwtProvider jwtProvider)
        {
            _authConfig = authConfig.Value;
            _metaDbContext = metaDbContext;
            _jwtProvider = jwtProvider;
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
