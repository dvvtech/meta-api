namespace MetaApi.Configuration.Auth
{
    public class MailRuAuthConfig
    {
        public const string SectionName = "MailRuAuth";

        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string RedirectUrl { get; init; }
        public string Scope { get; init; }
    }
}
