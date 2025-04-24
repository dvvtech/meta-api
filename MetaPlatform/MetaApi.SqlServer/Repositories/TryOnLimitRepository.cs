using MetaApi.Core.Domain.UserTryOnLimit;
using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{
    public interface ITryOnLimitRepository
    {
        Task<UserTryOnLimit> GetLimit(int userId);
        Task AddLimit(UserTryOnLimit userTryOnLimitEntity);
        Task UpdateLimit(UserTryOnLimit userTryOnLimitEntity);
    }

    public class TryOnLimitRepository : ITryOnLimitRepository
    {
        private readonly MetaDbContext _dbContext;

        public TryOnLimitRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserTryOnLimit> GetLimit(int userId)
        {
            var limit = await _dbContext.UserTryOnLimits.FirstOrDefaultAsync(l => l.AccountId == userId);

            return limit == null ? null : CreateLimitFromEntity(limit);
        }

        public async Task AddLimit(UserTryOnLimit limit)
        {
            var newLimit = new UserTryOnLimitEntity
            {
                AccountId = limit.AccountId,
                MaxAttempts = limit.MaxAttempts,
                AttemptsUsed = limit.AttemptsUsed,
                LastResetTime = limit.LastResetTime,
                ResetPeriod = limit.ResetPeriod
            };

            _dbContext.UserTryOnLimits.Add(newLimit);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateLimit(UserTryOnLimit userTryOnLimit)
        {
            await _dbContext.UserTryOnLimits.Where(limit => limit.Id == userTryOnLimit.Id)
                                            .ExecuteUpdateAsync(limit => limit
                                                .SetProperty(p => p.MaxAttempts, userTryOnLimit.MaxAttempts)
                                                .SetProperty(p => p.AttemptsUsed, userTryOnLimit.AttemptsUsed)
                                                .SetProperty(p => p.LastResetTime, userTryOnLimit.LastResetTime)
                                                .SetProperty(p => p.ResetPeriod, userTryOnLimit.ResetPeriod));
            await _dbContext.SaveChangesAsync();
        }

        private UserTryOnLimit CreateLimitFromEntity(UserTryOnLimitEntity entity)
        {
            return UserTryOnLimit.Create(entity.Id,
                                         entity.AccountId,
                                         entity.MaxAttempts,
                                         entity.AttemptsUsed,
                                         entity.LastResetTime,
                                         entity.ResetPeriod);
        }
    }
}
