using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Vk
{
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
