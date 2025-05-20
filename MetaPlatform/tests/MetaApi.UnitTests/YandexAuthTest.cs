using MetaApi.Configuration.Auth;
using MetaApi.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.UnitTests
{
    public class YandexAuthTest
    {
        [Fact]
        public async Task GetTimeUntilLimitResetAsync_ReturnsZero_WhenResetTimeHasPassed2()
        {
            ExchangeCodeForToken("gyxixskigavvpggt");
        }

        private async Task<YandexTokenResponse> ExchangeCodeForToken(string code)
        {
            using var httpClient = new HttpClient();

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", "ec3a2b886c544647881f54cd6535bb54"),
                new KeyValuePair<string, string>("client_secret", "04e8d9d7767c4edbb768fade5721c5be"),
                new KeyValuePair<string, string>("redirect_uri", "https://a33140-9deb.k.d-f.pw/api/yandex-auth/callback")
                });

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync("https://oauth.yandex.ru/token", requestContent);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await response.Content.ReadFromJsonAsync<YandexTokenResponse>();
        }
    }
}
