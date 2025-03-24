using Google.Apis.Auth.OAuth2.Flows;
using System.Net.Http.Headers;
using MetaApi.Models.Auth;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Responses;

namespace MetaApi.Services.Auth
{
    public partial class GoogleAuthService
    {
        /// <summary>
        /// После авторизации пользователяя гугл вызовет этот код и отправит code и мы его поменяем на токен
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<MetaApi.Models.Auth.TokenResponse> HandleCallback(string code)
        {
            try
            {
                var flow = new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = GetClientSecrets(),
                        Scopes = GetScopes()
                    });

                var tokenResponse = await flow.ExchangeCodeForTokenAsync("user", code, _authConfig.RedirectUrl, CancellationToken.None);
                GoogleUserInfo userInfo = await GetUserInfo(tokenResponse.AccessToken);

                string accessToken = _jwtProvider.GenerateToken(userInfo.GivenName, userInfo.Sub);
                string refreshToken = _jwtProvider.GenerateRefreshToken();

                var newUserEntity = new AccountEntity
                {
                    ExternalId = userInfo.Sub,
                    UserName = userInfo.GivenName,
                    JwtRefreshToken = refreshToken,
                    AuthType = AuthTypeEntity.Google,                    
                    Role = RoleEntity.User,
                    CreatedUtcDate = DateTime.UtcNow,
                    UpdateUtcDate = DateTime.UtcNow
                };

                AccountEntity userEntity = await _metaDbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(user => user.ExternalId == newUserEntity.ExternalId);
                if (userEntity == null)
                {
                    await _metaDbContext.Accounts.AddAsync(newUserEntity);
                    await _metaDbContext.SaveChangesAsync();
                }
                else
                {
                    //update refreshtoken
                    await _metaDbContext.Accounts
                                .Where(updateUser => updateUser.ExternalId == userEntity.ExternalId)
                                .ExecuteUpdateAsync(updateUser => updateUser
                                    .SetProperty(c => c.JwtRefreshToken, userEntity.JwtRefreshToken)
                                    .SetProperty(c => c.UpdateUtcDate, DateTime.UtcNow));
                }

                return new MetaApi.Models.Auth.TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GoogleUserInfo> GetUserInfo(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            response.EnsureSuccessStatusCode();
            var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
            return userInfo;
        }
    }
}
