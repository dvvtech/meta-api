namespace MetaApi.Services.AiClients.Base
{
    public interface IAiClient
    {
        Task<string> GetTextResponseAsync(string prompt, string systemPrompt);
    }
}
