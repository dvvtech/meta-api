namespace MetaApi.Configuration
{
    public class GoogleRecaptchaConfig
    {
        public const string SectionName = "GoogleRecaptcha";

        public string SecretKey { get; init; }
    }
}
