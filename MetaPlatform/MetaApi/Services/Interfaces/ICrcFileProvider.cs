namespace MetaApi.Services.Interfaces
{
    public interface ICrcFileProvider
    {
        IReadOnlyDictionary<string, string> FileCrcDictionary { get; }
        void AddFileCrc(string crc, string fileName);
    }
}
