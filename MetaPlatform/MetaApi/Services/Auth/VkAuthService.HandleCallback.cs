using MetaApi.SqlServer.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace MetaApi.Services
{
    public partial class VkAuthService
    {
        public async Task<MetaApi.Models.Auth.TokenResponse> HandleCallback(string code, string state, string deviceId)
        {
            try
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
                TokenResponse tokenResponse = await ExchangeCodeForToken(code, codeVerifier, deviceId);

                string userName = await GetUserName(tokenResponse.AccessToken, tokenResponse.RefreshToken, deviceId);

                string accessToken = _jwtProvider.GenerateToken(userName, tokenResponse.UserId.ToString());
                string refreshToken = _jwtProvider.GenerateRefreshToken();

                var newUserEntity = new AccountEntity
                {
                    ExternalId = tokenResponse.UserId.ToString(),
                    UserName = userName,
                    JwtRefreshToken = refreshToken,
                    AuthType = AuthTypeEntity.Vk,
                    Role = RoleEntity.User,
                    CreatedUtcDate = DateTime.UtcNow,
                    UpdateUtcDate = DateTime.UtcNow
                };

                AccountEntity accountEntity = await _accountRepository.GetByExternalId(newUserEntity.ExternalId);
                if (accountEntity == null)
                {
                    await _accountRepository.Add(newUserEntity);
                }
                else
                {
                    await _accountRepository.UpdateRefreshToken(accountEntity);
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

        private async Task<TokenResponse> ExchangeCodeForToken(string code, string codeVerifier, string deviceId)
        {            
            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _authConfig.ClientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _authConfig.RedirectUrl),
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

            return tokenResponse;
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
