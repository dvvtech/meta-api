using MetaApi.Core.Domain.UserTryOnLimit;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace MetaApi.Core.Services
{
    public class TryOnLimitService : ITryOnLimitService
    {
        private readonly ITryOnLimitRepository _repository;
        private readonly ISystemTime _systemTime;
        private readonly ILogger<TryOnLimitService> _logger;

        public TryOnLimitService(ITryOnLimitRepository cache,
                                 ISystemTime systemTime,
                                 ILogger<TryOnLimitService> logger)
        {
            _repository = cache;
            _systemTime = systemTime;
            _logger = logger;
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
            limit.TotalAttemptsUsed++;
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
        private void ResetLimitIfPeriodPassed(UserTryOnLimit limit)
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
        private async Task<UserTryOnLimit> GetOrCreateLimitAsync(int userId)
        {
            var limit = await _repository.GetLimit(userId);

            if (limit == null)
            {
                limit = UserTryOnLimit.Create(accountId: userId,
                                              maxAttempts: 3, // Дефолтное значение (можно вынести в конфиг)
                                              attemptsUsed: 0,
                                              lastResetTime: _systemTime.UtcNow,
                                              resetPeriod: TimeSpan.FromDays(1));

                await _repository.AddLimit(limit);
            }

            return limit;
        }
    }
}
