using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace MetaApi.Services
{
    public partial class VkAuthService
    {
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
    }
}
