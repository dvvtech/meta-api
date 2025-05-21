namespace MetaApi.Services.Auth
{
    public partial class YandexAuthService
    {        
        public string GenerateAuthUrl()
        {
            var queryParams = new Dictionary<string, string>
            {
                {"response_type", "code"},
                {"client_id", _authConfig.ClientId},
                {"redirect_uri", _authConfig.RedirectUrl},
                {"scope", "login:email login:info"}
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            return $"https://oauth.yandex.ru/authorize?{queryString}";
        }
    }
}
