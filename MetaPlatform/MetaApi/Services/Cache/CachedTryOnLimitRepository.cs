using MetaApi.SqlServer.Entities;
using MetaApi.SqlServer.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace MetaApi.Services.Cache
{
    public class CachedTryOnLimitRepository : ITryOnLimitRepository
    {
        private readonly ITryOnLimitRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CachedTryOnLimitRepository> _logger;

        public CachedTryOnLimitRepository(ITryOnLimitRepository repository,
                               IMemoryCache memoryCache,
                               ILogger<CachedTryOnLimitRepository> logger)
        {
            _repository = repository;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<UserTryOnLimitEntity> GetLimit(int userId)
        {
            var cacheKey = GetCacheKey(userId);

            // 1. Проверяем memory cache
            if (_memoryCache.TryGetValue(cacheKey, out UserTryOnLimitEntity cachedLimit))
            {
                _logger.LogDebug("Memory cache hit for user {UserId}", userId);
                return cachedLimit;
            }

            // 2. Если нет в memory cache, идем в БД
            _logger.LogDebug("Loading limit from DB for user {UserId}", userId);
            
            var limitEntity = await _repository.GetLimit(userId);
            if (limitEntity != null)
            {
                // 3. Сохраняем в memory cache
                SetCache(cacheKey, limitEntity);                
            }

            return limitEntity;
        }

        public async Task AddLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            await _repository.AddLimit(userTryOnLimitEntity);

            // Сохраняем в memory cache
            UpdateCache(userTryOnLimitEntity.AccountId, userTryOnLimitEntity);
        }

        public async Task UpdateLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            await _repository.UpdateLimit(userTryOnLimitEntity);

            // Обновляем кэш
            UpdateCache(userTryOnLimitEntity.AccountId, userTryOnLimitEntity);
        }

        #region Helper Methods

        private string GetCacheKey(int userId) => $"try_on_limit_{userId}";

        private void SetCache(string cacheKey, UserTryOnLimitEntity data)
        {
            _memoryCache.Set(cacheKey, data);
            _logger.LogDebug("Updated memory cache for key {CacheKey}", cacheKey);
        }

        private void UpdateCache(int userId, UserTryOnLimitEntity data)
        {
            var cacheKey = GetCacheKey(userId);
            SetCache(cacheKey, data);
        }

        #endregion
    }
}
