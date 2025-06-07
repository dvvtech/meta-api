using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace MetaApi.Services.Auth
{
    public partial class MailRuAuthService
    {
        public string GenerateAuthUrl()
        {
            var state = GenerateRandomString(32);
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            // Сохраняем code_verifier в кэше по state
            _cache.Set(state, codeVerifier, TimeSpan.FromMinutes(5));

            var authUrl = $"https://oauth.mail.ru/login?" +
                         $"client_id={_authConfig.ClientId}&" +
                         $"response_type=code&" +
                         $"scope={_authConfig.Scope}&" +
                         $"redirect_uri={Uri.EscapeDataString(_authConfig.RedirectUrl)}&" +
                         $"state={state}";

            return authUrl;
        }

        // Вспомогательные методы для PKCE (аналогичные VkAuthService)
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
    }
}
