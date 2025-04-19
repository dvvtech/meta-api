using MetaApi.SqlServer.Entities;
using MetaApi.SqlServer.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace MetaApi.Services.Cache
{
    public class TryOnLimitCache
    {
        private readonly ITryOnLimitRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TryOnLimitCache> _logger;

        public TryOnLimitCache(ITryOnLimitRepository repository,
                               IMemoryCache memoryCache,
                               ILogger<TryOnLimitCache> logger)
        {
            _repository = repository;
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
            
            var limitEntity = await _repository.GetLimit(userId);
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
            await _repository.AddLimit(userTryOnLimitEntity);            

            //Сохраняем в memory cache
            var cacheKey = $"try_on_limit_{userTryOnLimitEntity.Account}";
            _memoryCache.Set(cacheKey, userTryOnLimitEntity);
        }

        public async Task UpdateLimit(UserTryOnLimitEntity userTryOnLimitEntity)
        {
            await _repository.UpdateLimit(userTryOnLimitEntity);

            var cacheKey = $"try_on_limit_{userTryOnLimitEntity.AccountId}";
            _memoryCache.Set(cacheKey, userTryOnLimitEntity);            
        }
    }
}
