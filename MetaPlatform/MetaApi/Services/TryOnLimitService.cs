using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public class TryOnLimitService
    {
        private readonly MetaDbContext _context;

        public TryOnLimitService(MetaDbContext context)
        {
            _context = context;
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

            await _context.SaveChangesAsync();
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
        /// Устанавливает новый лимит (например, 3 попытки в час)
        /// </summary>
        public async Task SetLimitAsync(int userId, int maxAttempts, TimeSpan resetPeriod)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            limit.MaxAttempts = maxAttempts;
            limit.ResetPeriod = resetPeriod;
            await _context.SaveChangesAsync();
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
            var limit = await _context.UserTryOnLimits
                .FirstOrDefaultAsync(l => l.AccountId == userId);

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
                _context.UserTryOnLimits.Add(limit);
                await _context.SaveChangesAsync();
            }

            return limit;
        }
    }
}
