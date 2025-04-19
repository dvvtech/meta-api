using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{
    public interface ITryOnLimitRepository
    {
        Task<UserTryOnLimitEntity> GetLimit(int userId);
        Task AddLimit(UserTryOnLimitEntity userTryOnLimitEntity);
        Task UpdateLimit(UserTryOnLimitEntity userTryOnLimitEntity);
    }

    public class TryOnLimitRepository : ITryOnLimitRepository
    {
        private readonly MetaDbContext _dbContext;

        public TryOnLimitRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserTryOnLimitEntity> GetLimit(int userId)
        {
            return await _dbContext.UserTryOnLimits.FirstOrDefaultAsync(l => l.AccountId == userId);
        }

        public async Task AddLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            _dbContext.UserTryOnLimits.Add(userTryOnLimitEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            await _dbContext.UserTryOnLimits.Where(limit => limit.Id == userTryOnLimitEntity.Id)
                                            .ExecuteUpdateAsync(limit => limit
                                                .SetProperty(p => p.MaxAttempts, userTryOnLimitEntity.MaxAttempts)
                                                .SetProperty(p => p.AttemptsUsed, userTryOnLimitEntity.AttemptsUsed)
                                                .SetProperty(p => p.LastResetTime, userTryOnLimitEntity.LastResetTime)
                                                .SetProperty(p => p.ResetPeriod, userTryOnLimitEntity.ResetPeriod));
            await _dbContext.SaveChangesAsync();
        }
    }
}
