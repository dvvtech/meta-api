using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth.Mail
{
    public class MailRuErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }
}
