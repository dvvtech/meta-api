using MetaApi.Services.Cache;
using MetaApi.SqlServer.Entities;

namespace MetaApi.Services
{
    public class TryOnLimitService
    {        
        private readonly CachedTryOnLimitRepository _cache;

        public TryOnLimitService(CachedTryOnLimitRepository cache)
        {
            _cache = cache;
        }

        public async Task<int> GetRemainingUsage(int userId)
        {
            var userLimit = await _cache.GetLimit(userId);
            if (userLimit == null)
            {
                return 0;
            }

            return userLimit.MaxAttempts - userLimit.AttemptsUsed;
        }

        public async Task<TimeSpan> GetTimeUntilLimitResetAsync(int userId)
        {
            var userLimit = await _cache.GetLimit(userId);

            if (userLimit == null)
                return TimeSpan.Zero;

            // Вычисляем время следующего сброса
            DateTime nextResetTime = userLimit.LastResetTime + userLimit.ResetPeriod;

            // Текущее время (UTC, чтобы избежать проблем с часовыми поясами)
            DateTime currentTime = DateTime.UtcNow;

            // Если время сброса уже наступило, значит лимит уже сброшен (осталось 0 времени)
            if (currentTime >= nextResetTime)
            {
                return TimeSpan.Zero;
            }

            // Иначе возвращаем разницу между следующим сбросом и текущим временем
            return nextResetTime - currentTime;
        }

        /// <summary>
        /// Проверяет, может ли пользователь совершить попытку
        /// </summary>
        public async Task<bool> CanUserTryOnAsync(int userId)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            ResetLimitIfPeriodPassed(limit);

            return limit.AttemptsUsed < limit.MaxAttempts;
        }

        /// <summary>
        /// Уменьшает лимит после успешной попытки
        /// </summary>
        public async Task DecrementTryOnLimitAsync(int userId)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            ResetLimitIfPeriodPassed(limit);

            limit.AttemptsUsed++;
            limit.LastResetTime = DateTime.UtcNow;

            await _cache.UpdateLimit(limit);
        }

        /// <summary>
        /// Получает оставшиеся попытки
        /// </summary>
        public async Task<int> GetRemainingTriesAsync(int userId)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            ResetLimitIfPeriodPassed(limit);

            return limit.MaxAttempts - limit.AttemptsUsed;
        }        

        /// <summary>
        /// Сбрасывает счетчик, если прошел заданный период
        /// </summary>
        private void ResetLimitIfPeriodPassed(UserTryOnLimitEntity limit)
        {
            var now = DateTime.UtcNow;
            var timeSinceLastReset = now - limit.LastResetTime;

            if (timeSinceLastReset >= limit.ResetPeriod)
            {
                limit.AttemptsUsed = 0;
                limit.LastResetTime = now;
            }
        }


        /// <summary>
        /// Создает или получает лимит пользователя
        /// </summary>
        private async Task<UserTryOnLimitEntity> GetOrCreateLimitAsync(int userId)
        {
            var limit = await _cache.GetLimit(userId);

            if (limit == null)
            {
                limit = new UserTryOnLimitEntity
                {
                    AccountId = userId,
                    MaxAttempts = 3, // Дефолтное значение (можно вынести в конфиг)
                    AttemptsUsed = 0,
                    LastResetTime = DateTime.UtcNow,
                    ResetPeriod = TimeSpan.FromDays(1) // Дефолтный период (1 день)
                };

                await _cache.AddLimit(limit);
            }

            return limit;
        }
    }
}
