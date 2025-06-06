﻿using MetaApi.Configuration.Auth;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Options;

namespace MetaApi.Services.Auth
{
    public partial class GoogleAuthService
    {
        private readonly GoogleAuthConfig _authConfig;        
        private readonly IJwtProvider _jwtProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(IOptions<GoogleAuthConfig> authConfig,
                                 IAccountRepository accountRepository,                                 
                                 IJwtProvider jwtProvider,
                                 HttpClient httpClient,
                                 ILogger<GoogleAuthService> logger)
        {
            _authConfig = authConfig.Value;
            _accountRepository = accountRepository;            
            _jwtProvider = jwtProvider;
            _httpClient = httpClient;
            _logger = logger;
        }        
    }

    /*public class TokenResponse
    {
        public Guid UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public long? ExpiresInSeconds { get; set; }

        public string IdToken { get; set; }

        public DateTime IssuedUtc { get; set; }
    }*/
}
