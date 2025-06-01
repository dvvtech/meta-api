using Google.Apis.Auth.OAuth2.Flows;
using System.Net.Http.Headers;
using MetaApi.Models.Auth;
using MetaApi.Core.Domain.Account;

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

                string accessToken = string.Empty;
                string refreshToken = _jwtProvider.GenerateRefreshToken();
                
                Account account = await _accountRepository.GetByExternalId(userInfo.Sub);                
                if (account == null)
                {
                    var newUserEntity = Account.Create(externalId: userInfo.Sub,
                                                       userName: userInfo.GivenName,
                                                       email: "",
                                                       jwtRefreshToken: refreshToken,
                                                       authType: AuthType.Google,
                                                       role: Role.User);
                    
                    int accountId = await _accountRepository.Add(newUserEntity);
                    accessToken = _jwtProvider.GenerateToken(userInfo.GivenName, accountId);
                }
                else
                {
                    accessToken = _jwtProvider.GenerateToken(userInfo.GivenName, account.Id);
                    account.JwtRefreshToken = refreshToken;                    
                    await _accountRepository.UpdateRefreshToken(account);
                }

                return new MetaApi.Models.Auth.TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleCallback error");
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
