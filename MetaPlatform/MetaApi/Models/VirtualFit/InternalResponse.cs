using System.Text.Json.Serialization;

namespace MetaApi.Models.VirtualFit
{
    public class InternalResponse
    {
        [JsonPropertyName("output")]
        public string Output { get; set; }
    }
}
