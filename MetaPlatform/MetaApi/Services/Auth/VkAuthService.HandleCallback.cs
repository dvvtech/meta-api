using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MetaApi.Services
{
    public partial class VkAuthService
    {
        public async Task<MetaApi.Models.Auth.TokenResponse> HandleCallback(string code, string state, string deviceId)
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
            var authJson = await ExchangeCodeForToken(code, codeVerifier, deviceId);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(authJson);

            _logger.LogInformation($"token: {tokenResponse.AccessToken}");

            string userName = await GetUserName(tokenResponse.AccessToken, tokenResponse.RefreshToken, deviceId);

            string accessToken = _jwtProvider.GenerateToken(userName, tokenResponse.UserId.ToString());
            string refreshToken = _jwtProvider.GenerateRefreshToken();

            var newUserEntity = new AccountEntity
            {
                ExternalId = tokenResponse.UserId.ToString(),
                UserName = userName,
                JwtRefreshToken = refreshToken,
                AuthType = AuthTypeEntity.Vk,
//                AuthJson = authJson,
                Role = RoleEntity.User                
            };

            AccountEntity userEntity = await _metaDbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(user => user.ExternalId == newUserEntity.ExternalId);
            if (userEntity == null)
            {
                _metaDbContext.Accounts.AddAsync(userEntity);
            }

            return new MetaApi.Models.Auth.TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
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
            
            return responseContent;
        }

        public async Task<string> GetUserName(string accessToken, string refreshToken, string deviceId)
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

                if (userResponse?.Response.Count > 0)
                {
                    return userResponse?.Response[0].FirstName;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserName failed: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
