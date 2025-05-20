using MetaApi.Configuration.Auth;
using MetaApi.Core.Domain.Account;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace MetaApi.Services.Auth
{
    public partial class YandexAuthService
    {
        private readonly IMemoryCache _cache;
        private readonly YandexAuthConfig _authConfig;
        private readonly IJwtProvider _jwtProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<YandexAuthService> _logger;

        public YandexAuthService(IMemoryCache cache,
                                 IOptions<YandexAuthConfig> authConfig,
                                 IAccountRepository accountRepository,
                                 IJwtProvider jwtProvider,
                                 ILogger<YandexAuthService> logger)
        {
            _cache = cache;
            _authConfig = authConfig.Value;
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        /// <summary>
        /// Генерируем URL для авторизации через Яндекс
        /// </summary>
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

        /// <summary>
        /// Обрабатываем callback от Яндекс после авторизации
        /// </summary>
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
                    userName: userInfo.Login ?? userInfo.DefaultEmail,
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
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", accessToken);

            var response = await httpClient.GetAsync("https://login.yandex.ru/info?format=json");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<YandexUserInfo>();
        }
    }

    // Модели для ответов от Яндекс API
    public class YandexTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public class YandexUserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("real_name")]
        public string RealName { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("default_email")]
        public string DefaultEmail { get; set; }

        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; }
    }
}
