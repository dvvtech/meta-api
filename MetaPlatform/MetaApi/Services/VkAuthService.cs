using MetaApi.Controllers;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaApi.Services
{
    public class VkAuthService
    {
        private readonly IMemoryCache _cache;
        private const string ClientId = "53137675";//from config
        private const string RedirectUri = "https://a30944-8332.x.d-f.pw/api/vk-authorize/callback";
        private const string Scope = "email phone";//"email phone";

        private readonly ILogger<VkAuthService> _logger;

        public VkAuthService(IMemoryCache cache, ILogger<VkAuthService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        #region Generate login url

        public string GenerateAuthUrl()
        {
            var state = GenerateRandomString(32); // Генерация случайного state
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            // Сохраняем code_verifier в кэше по state
            _cache.Set(state, codeVerifier, TimeSpan.FromMinutes(5)); // Храним 5 минут

            var authUrl = $"https://id.vk.com/authorize?response_type=code&client_id={ClientId}&scope={Scope}&redirect_uri={RedirectUri}&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256";

            return authUrl;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateCodeVerifier()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Base64UrlEncode(randomBytes);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Base64UrlEncode(hash);
            }
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        #endregion

        #region Handle callback

        public async Task HandleCallback(string code, string state, string deviceId)
        {            
            // Извлекаем code_verifier из кэша по state
            if (!_cache.TryGetValue(state, out string codeVerifier))
            {
                throw new Exception("Invalid or expired state");
            }

            _logger.LogInformation($"code: {code} {Environment.NewLine}" +
                                   $"codeVerifier: {codeVerifier}  {Environment.NewLine}" +
                                   $"device_id: {deviceId}  {Environment.NewLine}");

            // Обмениваем код на токен
            var accessToken = await ExchangeCodeForToken(code, codeVerifier, deviceId);

            _logger.LogInformation($"token: {accessToken}");

            //ValidateToken(accessToken, "", "");


            // Дальнейшая логика (например, сохранение токена в базе данных)
        }

        private async Task<string> ExchangeCodeForToken(string code, string codeVerifier, string deviceId)
        {
            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", VkAuthService.ClientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", VkAuthService.RedirectUri),
                new KeyValuePair<string, string>("code_verifier", codeVerifier),
                new KeyValuePair<string, string>("device_id", deviceId)
            });

            var response = await client.PostAsync("https://id.vk.com/oauth2/auth", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Проверяем, есть ли ошибка в ответе
            if (responseContent.Contains("\"error\""))
            {
                var errorResponse = JsonSerializer.Deserialize<VkErrorResponse>(responseContent);
                //throw new Exception($"VK API Error: {errorResponse.Error}, Description: {errorResponse.ErrorDescription}");
            }

            _logger.LogInformation("token info:" + responseContent);

            // Парсим ответ и извлекаем access_token
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            return tokenResponse.AccessToken;
        }

        public async Task<bool> ValidateToken(string accessToken, string refreshToken, string deviceId)
        {
            string ApiUrl = "https://api.vk.com/method/";

            var client = new HttpClient();
            var requestUrl = $"{ApiUrl}users.get?access_token={accessToken}&v=5.131";

            try
            {
                var response = await client.GetStringAsync(requestUrl);
                _logger.LogInformation($"user response: {response}");

                if (response.Contains("\"error\""))
                {
                    var errorResponse = JsonSerializer.Deserialize<VkErrorResponse>(response);

                    // Если ошибка связана с IP-адресом, обновляем токен
                    /*if (errorResponse.ErrorCode == 5 && errorResponse.ErrorSubcode == 1130)
                    {
                        var newAccessToken = await RefreshToken(refreshToken, deviceId);
                        return await ValidateToken(newAccessToken, refreshToken, deviceId);
                    }*/

                    throw new Exception($"VK API Error: {errorResponse.Error}, Description: {errorResponse.ErrorDescription}");
                }                

                var userResponse = JsonSerializer.Deserialize<VkUserResponse>(response);


                return userResponse?.Response?.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }

        #endregion
    }

    public class VKAuthResponse
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string DeviceId { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public class VkErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }

    public class VkUserResponse
    {
        [JsonPropertyName("response")]
        public List<VkUser> Response { get; set; }
    }

    public class VkUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }

}
