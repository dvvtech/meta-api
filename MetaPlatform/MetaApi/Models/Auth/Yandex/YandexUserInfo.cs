using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Yandex
{
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
