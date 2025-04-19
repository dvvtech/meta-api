using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MetaApi.Services.Cache
{
    public class TryOnLimitCache
    {
        private readonly MetaDbContext _metaDbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TryOnLimitCache> _logger;

        public TryOnLimitCache(MetaDbContext dbContext,
                               IMemoryCache memoryCache,
                               ILogger<TryOnLimitCache> logger)
        {
            _metaDbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<UserTryOnLimitEntity> GetLimit(int userId)
        {
            var cacheKey = $"try_on_limit_{userId}";

            // 1. Проверяем memory cache
            if (_memoryCache.TryGetValue(cacheKey, out UserTryOnLimitEntity limitMemoryCached))
            {
                _logger.LogDebug("Memory cache hit for user {UserId}", userId);
                return limitMemoryCached;
            }

            // 2. Если нет в memory cache, идем в БД
            _logger.LogDebug("Loading limit from DB for user {UserId}", userId);

            var limitEntity = await _metaDbContext.UserTryOnLimits.FirstOrDefaultAsync(l => l.AccountId == userId);
            if (limitEntity != null)
            {
                // 3. Сохраняем в memory cache
                _memoryCache.Set(cacheKey, limitEntity);
                return limitEntity;
            }

            return null;
        }

        public async Task AddLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            _metaDbContext.UserTryOnLimits.Add(userTryOnLimitEntity);
            await _metaDbContext.SaveChangesAsync();

            //Сохраняем в memory cache
            var cacheKey = $"try_on_limit_{userTryOnLimitEntity.Account}";
            _memoryCache.Set(cacheKey, userTryOnLimitEntity);
        }

        public async Task UpdateLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            await _metaDbContext.UserTryOnLimits.Where(limit => limit.Id == userTryOnLimitEntity.Id)
                                                .ExecuteUpdateAsync(limit => limit
                                                    .SetProperty(p => p.MaxAttempts, userTryOnLimitEntity.MaxAttempts)
                                                    .SetProperty(p => p.AttemptsUsed, userTryOnLimitEntity.AttemptsUsed)
                                                    .SetProperty(p => p.LastResetTime, userTryOnLimitEntity.LastResetTime)
                                                    .SetProperty(p => p.ResetPeriod, userTryOnLimitEntity.ResetPeriod));
            await _metaDbContext.SaveChangesAsync();

            var cacheKey = $"try_on_limit_{userTryOnLimitEntity.AccountId}";
            _memoryCache.Set(cacheKey, userTryOnLimitEntity);            
        }
    }
}
