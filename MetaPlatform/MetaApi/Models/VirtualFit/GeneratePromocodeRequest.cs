namespace MetaApi.Models.VirtualFit
{    
    public class GeneratePromocodeRequest
    {
        /// <summary>
        /// // Имя клиента (опционально)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Админский промокод //todo вынести в конфиг
        /// </summary>
        public string AdminPromocode { get; set; }

        public int UsageLimit { get; set; } // Лимит использования промокода
    }
}
