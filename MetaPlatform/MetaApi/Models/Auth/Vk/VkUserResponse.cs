using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Vk
{
    public class VkUserResponse
    {
        [JsonPropertyName("response")]
        public List<VkUser> Response { get; set; }
    }
}
