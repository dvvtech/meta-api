using MetaApi.Core.Domain.Account;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

namespace MetaApi.Services.Auth
{
    public partial class YandexAuthService
    {        
        public async Task<TokenResponse> HandleCallback(string code)
        {
            // Ключ для кеша
            var cacheKey = $"yandex_auth_{code}";

            // Если задача уже есть в кеше, возвращаем её (await дождётся её завершения)
            if (_cache.TryGetValue(cacheKey, out Task<TokenResponse> ongoingTask))
            {
                return await ongoingTask;
            }

            // Создаём новую задачу, но пока не запускаем её
            var taskCompletionSource = new TaskCompletionSource<TokenResponse>();

            // Помещаем задачу в кеш (если другой поток уже добавил задачу, берём её)
            ongoingTask = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Кешируем на 5 минут
                return taskCompletionSource.Task;
            });

            // Если это первый вызов (taskCompletionSource ещё не завершён), обрабатываем запрос
            if (ongoingTask == taskCompletionSource.Task)
            {
                _logger.LogInformation("Auth proccess");
                try
                {
                    var result = await ProcessAuthCallback(code);
                    taskCompletionSource.SetResult(result); // Уведомляем все ожидающие запросы
                    return result;
                }
                catch (Exception ex)
                {
                    _cache.Remove(cacheKey); // Удаляем из кеша в случае ошибки
                    taskCompletionSource.SetException(ex); // Пробрасываем ошибку всем ожидающим
                    throw;
                }
            }

            // Если задача уже была в кеше, просто ждём её
            return await ongoingTask;
        }

        private async Task<TokenResponse> ProcessAuthCallback(string code)
        {
            // Получаем access token от Яндекс
            var tokenResponse = await ExchangeCodeForToken(code);

            // Получаем информацию о пользователе
            var userInfo = await GetUserInfo(tokenResponse.AccessToken);

            string accessToken = string.Empty;
            string refreshToken = _jwtProvider.GenerateRefreshToken();

            // Ищем или создаем пользователя в нашей системе
            Account account = await _accountRepository.GetByExternalId(userInfo.Id);

            if (account == null)
            {
                var newUserEntity = Account.Create(
                    externalId: userInfo.Id,
                    userName: userInfo.Login ?? "",
                    email: userInfo.DefaultEmail,
                    jwtRefreshToken: refreshToken,
                    authType: AuthType.Yandex,
                    role: Role.User);

                int accountId = await _accountRepository.Add(newUserEntity);
                accessToken = _jwtProvider.GenerateToken(newUserEntity.UserName, accountId);

                _logger.LogInformation($"Success register user with name: {userInfo?.Login}");
            }
            else
            {
                accessToken = _jwtProvider.GenerateToken(account.UserName, account.Id);
                account.JwtRefreshToken = refreshToken;
                await _accountRepository.UpdateRefreshToken(account);

                _logger.LogInformation($"Success yandex auth with name: {userInfo?.Login}");
            }

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<YandexTokenResponse> ExchangeCodeForToken(string code)
        {
            using var httpClient = new HttpClient();

            var requestContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", _authConfig.ClientId),
            new KeyValuePair<string, string>("client_secret", _authConfig.ClientSecret),
            new KeyValuePair<string, string>("redirect_uri", _authConfig.RedirectUrl)
        });

            var response = await httpClient.PostAsync("https://oauth.yandex.ru/token", requestContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<YandexTokenResponse>();
        }

        private async Task<YandexUserInfo> GetUserInfo(string accessToken)
        {            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", accessToken);

            var response = await _httpClient.GetAsync("https://login.yandex.ru/info?format=json");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<YandexUserInfo>();
        }
    }
}
