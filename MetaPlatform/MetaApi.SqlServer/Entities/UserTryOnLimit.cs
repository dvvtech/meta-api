
namespace MetaApi.SqlServer.Entities
{    
    public class UserTryOnLimitEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ на AccountEntity
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Навигационное свойство
        /// </summary>
        public AccountEntity Account { get; set; }

        /// <summary>
        /// Максимальное количество попыток
        /// </summary>
        public int MaxAttempts { get; set; }

        /// <summary>
        /// Использованные попытки
        /// </summary>
        public int AttemptsUsed { get; set; }

        /// <summary>
        /// Общее кол-во попыток
        /// </summary>
        public int TotalAttemptsUsed { get; set; }

        /// <summary>
        /// Время последнего сброса
        /// </summary>
        public DateTime LastResetTime { get; set; }

        /// <summary>
        /// Период сброса (например, 1 час, 1 день и т. д.)
        /// </summary>
        public TimeSpan ResetPeriod { get; set; } 
    }
}
