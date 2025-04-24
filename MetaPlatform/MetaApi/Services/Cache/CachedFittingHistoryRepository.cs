using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace MetaApi.Services.Cache
{
    public class CachedFittingHistoryRepository : IFittingHistoryRepository
    {
        private readonly IFittingHistoryRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CachedFittingHistoryRepository> _logger;
        
        public CachedFittingHistoryRepository(IFittingHistoryRepository repository,
                                              IMemoryCache memoryCache,
                                              ILogger<CachedFittingHistoryRepository> logger)
        {
            _repository = repository;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<FittingHistory[]> GetHistoryAsync(int accountId)
        {            
            var cacheKey = GetCacheKey(accountId);

            // 1. Проверяем memory cache
            if (_memoryCache.TryGetValue(cacheKey, out FittingHistory[] cachedData))
            {
                _logger.LogDebug("Memory cache hit for user {UserId}", accountId);
                return cachedData;
            }

            // 2. Если нет в memory cache, идем в БД
            _logger.LogDebug("Loading history from DB for user {UserId}", accountId);
            FittingHistory[] dbResults = await _repository.GetHistoryAsync(accountId);

            //3.Сохраняем в memory cache
            SetCache(cacheKey, dbResults);

            return dbResults;
        }

        public async Task<int> AddToHistoryAsync(FittingHistory item)
        {
            // 1. Сначала сохраняем в БД
            int itemId = await _repository.AddToHistoryAsync(item);

            // Обновляем кэш
            UpdateCacheAfterAddition(item.AccountId,
                                     FittingHistory.Create(id: itemId,
                                                           accountId: item.AccountId,
                                                           garmentImgUrl: item.GarmentImgUrl,
                                                           humanImgUrl: item.HumanImgUrl,
                                                           resultImgUrl: item.ResultImgUrl));

            return itemId;
        }

        public async Task DeleteAsync(int fittingResultId, int userId)
        {
            // 1. Находим и "удаляем" запись (помечаем IsDeleted)
            await _repository.DeleteAsync(fittingResultId, userId);

            // 2. Обновляем кеш                                    
            UpdateCacheAfterDeletion(userId, fittingResultId);
        }        

        #region Helper Methods

        private string GetCacheKey(int userId) => $"fitting_history_{userId}";

        private void SetCache(string cacheKey, FittingHistory[] data)
        {
            _memoryCache.Set(cacheKey, data);
            _logger.LogDebug("Updated memory cache for key {CacheKey}", cacheKey);
        }

        private void UpdateCacheAfterAddition(int userId, FittingHistory newItem)
        {
            var cacheKey = GetCacheKey(userId);

            if (_memoryCache.TryGetValue(cacheKey, out FittingHistory[] cachedData))
            {
                var updatedData = cachedData.Append(newItem).ToArray();
                SetCache(cacheKey, updatedData);
            }
            else
            {
                InvalidateCache(cacheKey);
            }
        }

        private void UpdateCacheAfterDeletion(int userId, int deletedId)
        {
            var cacheKey = GetCacheKey(userId);

            if (_memoryCache.TryGetValue(cacheKey, out FittingHistory[] cachedData))
            {
                var updatedData = cachedData.Where(x => x.Id != deletedId).ToArray();
                SetCache(cacheKey, updatedData);
            }
            else
            {
                _logger.LogDebug("No cache to update for user {UserId}", userId);
            }
        }

        private void InvalidateCache(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("Invalidated memory cache for key {CacheKey}", cacheKey);
        }

        #endregion
    }
}
