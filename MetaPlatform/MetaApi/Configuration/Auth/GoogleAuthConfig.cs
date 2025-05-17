namespace MetaApi.Configuration.Auth
{
    public class GoogleAuthConfig
    {
        public const string SectionName = "GoogleAuth";

        public string ClientId { get; init; }

        public string ClientSecret { get; init; }

        public string ApplicationName { get; init; }

        public string RedirectUrl { get; init; }        
    }
}
