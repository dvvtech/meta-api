namespace MetaApi.Models.VirtualFit
{    
    public class GeneratePromocodeRequest
    {
        public string Name { get; set; } // Имя человека (опционально)

        public int UsageLimit { get; set; } // Лимит использования промокода
    }
}
