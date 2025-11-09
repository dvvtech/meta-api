using MetaApi.Core.Domain.Account;
using MetaApi.Models.Auth;
using MetaApi.Models.Auth.Gazprom;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MetaApi.Services.Auth
{
    //public partial class GazpromIdAuthService
    //{
    //    public async Task<TokenResponse> HandleCallback(string code, string state)
    //    {
    //        if (!_cache.TryGetValue(state, out string codeVerifier))
    //            throw new Exception("Invalid or expired state");

    //        var tokenResponse = await ExchangeCodeForToken(code, codeVerifier);
    //        var userInfo = await GetUserInfo(tokenResponse.AccessToken);

    //        var (accessToken, refreshToken) = await ProcessUserAuth(userInfo);

    //        return new TokenResponse
    //        {
    //            AccessToken = accessToken,
    //            RefreshToken = refreshToken
    //        };
    //    }

    //    private async Task<GazpromTokenResponse> ExchangeCodeForToken(string code, string codeVerifier)
    //    {
    //        var requestContent = new FormUrlEncodedContent(new[]
    //        {
    //            new KeyValuePair<string, string>("client_id", _authConfig.ClientId),
    //            new KeyValuePair<string, string>("client_secret", _authConfig.ClientSecret),
    //            new KeyValuePair<string, string>("grant_type", "authorization_code"),
    //            new KeyValuePair<string, string>("code", code),
    //            new KeyValuePair<string, string>("redirect_uri", _authConfig.RedirectUrl),
    //            new KeyValuePair<string, string>("code_verifier", codeVerifier)
    //        });

    //        var response = await _httpClient.PostAsync(_authConfig.TokenEndpoint, requestContent);
    //        var responseContent = await response.Content.ReadAsStringAsync();

    //        if (!response.IsSuccessStatusCode)
    //            throw new Exception($"Gazprom ID token error: {responseContent}");

    //        return JsonSerializer.Deserialize<GazpromTokenResponse>(responseContent);
    //    }

    //    private async Task<GazpromUserInfo> GetUserInfo(string accessToken)
    //    {
    //        _httpClient.DefaultRequestHeaders.Authorization =
    //            new AuthenticationHeaderValue("Bearer", accessToken);

    //        var response = await _httpClient.GetStringAsync(_authConfig.UserInfoEndpoint);
    //        return JsonSerializer.Deserialize<GazpromUserInfo>(response);
    //    }

    //    private async Task<(string accessToken, string refreshToken)> ProcessUserAuth(GazpromUserInfo userInfo)
    //    {
    //        var account = await _accountRepository.GetByExternalId(userInfo.Sub);
    //        var refreshToken = _jwtProvider.GenerateRefreshToken();

    //        if (account == null)
    //        {
    //            var newAccount = Account.Create(
    //                userInfo.Sub,
    //                userInfo.Name ?? userInfo.PreferredUsername,
    //                userInfo.Email,
    //                refreshToken,
    //                AuthType.GazpromId,
    //                Role.User);

    //            var accountId = await _accountRepository.Add(newAccount);
    //            return (_jwtProvider.GenerateToken(newAccount.UserName, accountId), refreshToken);
    //        }
    //        else
    //        {
    //            account.JwtRefreshToken = refreshToken;
    //            await _accountRepository.UpdateRefreshToken(account);
    //            return (_jwtProvider.GenerateToken(account.UserName, account.Id), refreshToken);
    //        }
    //    }
    //}
}
