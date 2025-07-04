﻿using MetaApi.Core.Domain.Account;
using MetaApi.Models.Auth;
using MetaApi.Models.Auth.Mail;
using System.Text.Json;

namespace MetaApi.Services.Auth
{
    public partial class MailRuAuthService
    {
        public async Task<TokenResponse> HandleCallback(string code, string state)
        {
            try
            {                                
                // Обмениваем код на токен
                MailRuTokenResponse tokenResponse = await ExchangeCodeForToken(code);
                
                // Получаем информацию о пользователе
                MailRuUserInfo userInfo = await GetUserInfo(tokenResponse.AccessToken);
                
                string accessToken = string.Empty;
                string refreshToken = _jwtProvider.GenerateRefreshToken();
                
                Account account = await _accountRepository.GetByExternalId(userInfo.ExternalId);
                if (account == null)
                {
                    var newUserEntity = Account.Create(
                        externalId: userInfo.ExternalId,
                        userName: userInfo.Name,
                        email: userInfo.Email,
                        jwtRefreshToken: refreshToken,
                        authType: AuthType.MailRu,
                        role: Role.User);

                    int accountId = await _accountRepository.Add(newUserEntity);
                    accessToken = _jwtProvider.GenerateToken(userInfo.Name, accountId);
                }
                else
                {
                    accessToken = _jwtProvider.GenerateToken(userInfo.Name, account.Id);
                    account.JwtRefreshToken = refreshToken;
                    await _accountRepository.UpdateRefreshToken(account);
                }
                
                return new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MailRu HandleCallback error");
                throw;
            }
        }

        private async Task<MailRuTokenResponse> ExchangeCodeForToken(string code)
        {
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _authConfig.ClientId),
                new KeyValuePair<string, string>("client_secret", _authConfig.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _authConfig.RedirectUrl)
            });

            // Добавляем User-Agent
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Virtual-fit/1.0 (MailRu OAuth Client)");

            var response = await _httpClient.PostAsync("https://oauth.mail.ru/token", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<MailRuErrorResponse>(responseContent);
                throw new Exception($"Mail.ru API Error: {errorResponse?.Error}, Description: {errorResponse?.ErrorDescription}");
            }

            _logger.LogInformation("Mail.ru token info:" + responseContent);

            return JsonSerializer.Deserialize<MailRuTokenResponse>(responseContent);
        }

        private async Task<MailRuUserInfo> GetUserInfo(string accessToken)
        {
            var requestUrl = $"https://oauth.mail.ru/userinfo?access_token={accessToken}";

            var response = await _httpClient.GetStringAsync(requestUrl);
            _logger.LogInformation($"Mail.ru user response: {response}");

            if (response.Contains("\"error\""))
            {
                var errorResponse = JsonSerializer.Deserialize<MailRuErrorResponse>(response);
                throw new Exception($"Mail.ru API Error: {errorResponse.Error}, Description: {errorResponse.ErrorDescription}");
            }

            return JsonSerializer.Deserialize<MailRuUserInfo>(response);
        }
    }
}
