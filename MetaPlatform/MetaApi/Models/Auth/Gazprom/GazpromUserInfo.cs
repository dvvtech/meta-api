using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Gazprom
{
    public class GazpromUserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }  // Уникальный идентификатор пользователя

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }
    }
}
