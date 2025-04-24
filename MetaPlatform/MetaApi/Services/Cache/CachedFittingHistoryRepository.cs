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
            var cacheKey = $"fitting_history_{accountId}";

            // 1. Проверяем memory cache
            if (_memoryCache.TryGetValue(cacheKey, out FittingHistory[] memoryCached))
            {
                _logger.LogDebug("Memory cache hit for user {UserId}", accountId);
                return memoryCached;
            }

            // 2. Если нет в memory cache, идем в БД
            _logger.LogDebug("Loading history from DB for user {UserId}", accountId);
            FittingHistory[] dbResults = await _repository.GetHistoryAsync(accountId);            
            
            // 3. Сохраняем в memory cache            
            _memoryCache.Set(cacheKey, dbResults);

            return dbResults;
        }

        public async Task<int> AddToHistoryAsync(FittingHistory item)
        {
            // 1. Сначала сохраняем в БД
            int itemId = await _repository.AddToHistoryAsync(item);            

            // 2. Пытаемся получить текущий кеш
            var cacheKey = $"fitting_history_{item.AccountId}";            
            if (_memoryCache.TryGetValue(cacheKey, out FittingHistory[] cachedHistory))
            {
                // 3. Если кеш существует - обновляем его
                var updatedHistory = cachedHistory
                    .Append(FittingHistory.Create(id: itemId,
                                                  accountId: item.AccountId,
                                                  garmentImgUrl: item.GarmentImgUrl,
                                                  humanImgUrl: item.HumanImgUrl,
                                                  resultImgUrl: item.ResultImgUrl))                      
                    .ToArray();

                _memoryCache.Set(cacheKey, updatedHistory);
                _logger.LogDebug("Updated memory cache for user {UserId}", item.AccountId);
            }
            else
            {
                // 4. Если кеша нет - просто инвалидируем (при следующем GetHistory кеш заполнится)
                _memoryCache.Remove(cacheKey);
                _logger.LogDebug("Invalidated memory cache for user {UserId}", item.AccountId);
            }

            return itemId;
        }

        public async Task DeleteAsync(int fittingResultId, int userId)
        {
            // 1. Находим и "удаляем" запись (помечаем IsDeleted)
            await _repository.DeleteAsync(fittingResultId, userId);

            // 2. Обновляем кеш                        
            var cacheKey = $"fitting_history_{userId}";            
            await UpdateCacheAfterDeletion(userId, fittingResultId);
        }

        private async Task UpdateCacheAfterDeletion(int userId, int deletedId)
        {
            var cacheKey = $"fitting_history_{userId}";

            if (_memoryCache.TryGetValue(cacheKey, out FittingHistory[] cachedData))
            {
                var updatedData = cachedData
                    .Where(x => x.Id != deletedId)
                    .ToArray();

                _memoryCache.Set(cacheKey, updatedData);
                _logger.LogDebug("Cache updated after deletion for user {UserId}", userId);
            }
            else
            {
                // Если кеша нет - ничего не делаем, он загрузится при следующем запросе
                _logger.LogDebug("No cache to update for user {UserId}", userId);
            }
        }
    }
}
