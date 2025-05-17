namespace MetaApi.Configuration.Auth
{
    public class YandexAuthConfig
    {
        public const string SectionName = "YandexAuth";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
    }
}
