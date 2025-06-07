using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Vk
{
    public class VKAuthResponse
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string DeviceId { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public class VkErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }

    public class VkUserResponse
    {
        [JsonPropertyName("response")]
        public List<VkUser> Response { get; set; }
    }

    public class VkUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
}
