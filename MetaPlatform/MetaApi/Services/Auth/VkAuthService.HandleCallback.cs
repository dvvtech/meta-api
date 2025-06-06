using MetaApi.Core.Domain.Account;
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
                string userEmail = tokenResponse.Email ?? "";

                string accessToken = string.Empty;
                string refreshToken = _jwtProvider.GenerateRefreshToken();
                string externalId = tokenResponse.UserId.ToString();

                Account account = await _accountRepository.GetByExternalId(externalId);
                if (account == null)
                {
                    var newUserEntity = Account.Create(externalId: externalId,
                                                       userName: userName,
                                                       email: userEmail,
                                                       jwtRefreshToken: refreshToken,
                                                       authType: AuthType.Vk,
                                                       role: Role.User);                    

                    int accountId = await _accountRepository.Add(newUserEntity);
                    accessToken = _jwtProvider.GenerateToken(userName, accountId);
                }
                else
                {
                    accessToken = _jwtProvider.GenerateToken(userName, account.Id);
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

        private async Task<TokenResponse> ExchangeCodeForToken(string code, string codeVerifier, string deviceId)
        {                        
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _authConfig.ClientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _authConfig.RedirectUrl),
                new KeyValuePair<string, string>("code_verifier", codeVerifier),
                new KeyValuePair<string, string>("device_id", deviceId)
            });

            var response = await _httpClient.PostAsync("https://id.vk.com/oauth2/auth", requestContent);
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
            string apiUrl = "https://api.vk.com/method/";
            
            var requestUrl = $"{apiUrl}users.get?access_token={accessToken}&v=5.131";

            try
            {
                var response = await _httpClient.GetStringAsync(requestUrl);
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
