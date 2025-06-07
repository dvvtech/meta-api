
namespace MetaApi.Models.Auth.Vk
{
    public class VKAuthResponse
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string DeviceId { get; set; }
    }                
}
