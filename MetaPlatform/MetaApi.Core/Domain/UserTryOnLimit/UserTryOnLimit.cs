
namespace MetaApi.Core.Domain.UserTryOnLimit
{
    public class UserTryOnLimit
    {
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ на AccountEntity
        /// </summary>
        public int AccountId { get; set; }

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

        private UserTryOnLimit(int accountId, int maxAttempts, int attemptsUsed, DateTime lastResetTime, TimeSpan resetPeriod)
        {            
            AccountId = accountId;
            MaxAttempts = maxAttempts;
            AttemptsUsed = attemptsUsed;
            LastResetTime = lastResetTime;
            ResetPeriod = resetPeriod;
        }

        private UserTryOnLimit(int id, int accountId, int maxAttempts, int attemptsUsed, int totalAttemptsUsed, DateTime lastResetTime, TimeSpan resetPeriod)
        {
            Id = id;
            AccountId = accountId;
            MaxAttempts = maxAttempts;
            AttemptsUsed = attemptsUsed;
            TotalAttemptsUsed = totalAttemptsUsed;
            LastResetTime = lastResetTime;
            ResetPeriod = resetPeriod;
        }

        public static UserTryOnLimit Create(int accountId, int maxAttempts, int attemptsUsed, DateTime lastResetTime, TimeSpan resetPeriod)
        {
            return new UserTryOnLimit(accountId, maxAttempts, attemptsUsed, lastResetTime, resetPeriod);
        }

        public static UserTryOnLimit Create(int id, int accountId, int maxAttempts, int attemptsUsed, int totalAttemptsUsed, DateTime lastResetTime, TimeSpan resetPeriod)
        {
            return new UserTryOnLimit(id, accountId, maxAttempts, attemptsUsed, totalAttemptsUsed, lastResetTime, resetPeriod);
        }
    }
}
