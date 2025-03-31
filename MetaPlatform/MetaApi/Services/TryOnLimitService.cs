using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace MetaApi.Services
{
    public class TryOnLimitService
    {
        private readonly MetaDbContext _context;

        public TryOnLimitService(MetaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanUserTryOnAsync(int userId)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            UpdateDailyLimitIfNeeded(limit);

            return limit.TriesUsedToday < limit.DailyLimit;
        }

        public async Task DecrementTryOnLimitAsync(int userId)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            UpdateDailyLimitIfNeeded(limit);

            limit.TriesUsedToday++;
            limit.LastTryDate = DateTime.UtcNow.Date;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRemainingTriesAsync(int userId)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            UpdateDailyLimitIfNeeded(limit);

            return limit.DailyLimit - limit.TriesUsedToday;
        }

        public async Task SetDailyLimitAsync(int userId, int newLimit)
        {
            var limit = await GetOrCreateLimitAsync(userId);
            limit.DailyLimit = newLimit;
            await _context.SaveChangesAsync();
        }

        private async Task<UserTryOnLimitEntity> GetOrCreateLimitAsync(int userId)
        {
            var limit = await _context.UserTryOnLimits
                .FirstOrDefaultAsync(l => l.AccountId == userId);

            if (limit == null)
            {
                limit = new UserTryOnLimitEntity
                {
                    AccountId = userId,
                    DailyLimit = 3,
                    TriesUsedToday = 0,
                    LastTryDate = DateTime.UtcNow.Date.AddDays(-1)
                };
                _context.UserTryOnLimits.Add(limit);
                await _context.SaveChangesAsync();
            }

            return limit;
        }

        private void UpdateDailyLimitIfNeeded(UserTryOnLimitEntity limit)
        {
            var today = DateTime.UtcNow.Date;

            if (limit.LastTryDate < today)
            {
                limit.TriesUsedToday = 0;
                limit.LastTryDate = today;
            }
        }
    }
}
