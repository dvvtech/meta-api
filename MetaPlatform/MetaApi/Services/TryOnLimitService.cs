using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Services.Interfaces;
using MetaApi.SqlServer.Entities;
using MetaApi.SqlServer.Repositories;

namespace MetaApi.Services
{    
    public class TryOnLimitService : ITryOnLimitService
    {        
        private readonly ITryOnLimitRepository _repository;
        private readonly ISystemTime _systemTime;

        public TryOnLimitService(ITryOnLimitRepository cache, ISystemTime systemTime)
        {
            _repository = cache;
            _systemTime = systemTime;
        }

        public async Task<int> GetRemainingUsage(int userId)
        {
            var userLimit = await _repository.GetLimit(userId);
            if (userLimit == null)
            {
                return 0;
            }

            return userLimit.MaxAttempts - userLimit.AttemptsUsed;
        }

        public async Task<TimeSpan> GetTimeUntilLimitResetAsync(int userId)
        {
            var userLimit = await _repository.GetLimit(userId);

            if (userLimit == null)
                return TimeSpan.Zero;

            // Вычисляем время следующего сброса
            DateTime nextResetTime = userLimit.LastResetTime + userLimit.ResetPeriod;

            // Текущее время (UTC, чтобы избежать проблем с часовыми поясами)
            DateTime currentTime = _systemTime.UtcNow;

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
            limit.LastResetTime = _systemTime.UtcNow;

            await _repository.UpdateLimit(limit);
        }

        /// <summary>
        /// Возвращает оставшиеся попытки
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
            var now = _systemTime.UtcNow;
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
            var limit = await _repository.GetLimit(userId);

            if (limit == null)
            {
                limit = new UserTryOnLimitEntity
                {
                    AccountId = userId,
                    MaxAttempts = 3, // Дефолтное значение (можно вынести в конфиг)
                    AttemptsUsed = 0,
                    LastResetTime = _systemTime.UtcNow,
                    ResetPeriod = TimeSpan.FromDays(1) // Дефолтный период (1 день)
                };

                await _repository.AddLimit(limit);
            }

            return limit;
        }
    }    
}
