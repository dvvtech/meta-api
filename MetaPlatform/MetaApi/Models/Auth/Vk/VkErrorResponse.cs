using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Vk
{
    public class VkErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }
}
