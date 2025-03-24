using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;

namespace MetaApi.Services.Auth
{
    public partial class GoogleAuthService
    {
        /// <summary>
        /// Возвращаем пользователю урл для авторизации в гугл
        /// </summary>
        /// <returns></returns>
        public string GenerateAuthUrl()
        {
            return new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GetClientSecrets(),
                    Scopes = GetScopes(),
                    Prompt = "consent"
                }).CreateAuthorizationCodeRequest(_authConfig.RedirectUrl).Build().ToString();
        }

        private ClientSecrets GetClientSecrets()
        {
            string clientId = _authConfig.ClientId;
            string clientSecret = _authConfig.ClientSecret;

            return new() { ClientId = clientId, ClientSecret = clientSecret };
        }

        private string[] GetScopes()
        {
            return new[]
            {
                Oauth2Service.Scope.Openid,
                Oauth2Service.Scope.UserinfoEmail,
                Oauth2Service.Scope.UserinfoProfile,
            };
        }
    }
}
