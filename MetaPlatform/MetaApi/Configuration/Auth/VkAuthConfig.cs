namespace MetaApi.Configuration.Auth
{
    public class VkAuthConfig
    {
        public const string SectionName = "VkAuth";

        public string ClientId { get; init; }

        public string RedirectUrl { get; init; }

        public string Scope { get; init; }
    }
}
