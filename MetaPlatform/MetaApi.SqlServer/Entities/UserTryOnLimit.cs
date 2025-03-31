
namespace MetaApi.SqlServer.Entities
{
    //todo хранить часовой пояс чтобы пересчет кол-ва выполнялся относительно часового пояса клиента

    public class UserTryOnLimitEntity
    {
        public int Id { get; set; }
        public int AccountId { get; set; } 
        public AccountEntity User { get; set; }

        /// <summary>
        /// // Стандартный лимит - 3
        /// </summary>
        public int DailyLimit { get; set; } = 3;

        /// <summary>
        /// Кол-во использованных попыток за день
        /// </summary>
        public int TriesUsedToday { get; set; }

        /// <summary>
        /// Дата последней попытки
        /// </summary>
        public DateTime LastTryDate { get; set; } 
    }
}
