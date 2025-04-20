using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using MetaApi.SqlServer.Repositories;
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

        public async Task<FittingResultEntity[]> GetHistoryAsync(int userId)
        {
            var cacheKey = $"fitting_history_{userId}";

            // 1. Проверяем memory cache
            if (_memoryCache.TryGetValue(cacheKey, out FittingResultEntity[] memoryCached))
            {
                _logger.LogDebug("Memory cache hit for user {UserId}", userId);
                return memoryCached;
            }

            // 2. Если нет в memory cache, идем в БД
            _logger.LogDebug("Loading history from DB for user {UserId}", userId);
            var dbResults = await _repository.GetHistoryAsync(userId);            
            
            // 3. Сохраняем в memory cache            
            _memoryCache.Set(cacheKey, dbResults);

            return dbResults;
        }

        public async Task AddToHistoryAsync(FittingResultEntity entity)
        {
            // 1. Сначала сохраняем в БД
            await _repository.AddToHistoryAsync(entity);            

            // 2. Пытаемся получить текущий кеш
            var cacheKey = $"fitting_history_{entity.AccountId}";            
            if (_memoryCache.TryGetValue(cacheKey, out FittingHistoryResponse[] cachedHistory))
            {
                // 3. Если кеш существует - обновляем его
                var updatedHistory = cachedHistory
                    .Append(new FittingHistoryResponse
                    {
                        Id = entity.Id,
                        GarmentImgUrl = entity.GarmentImgUrl,
                        HumanImgUrl = entity.HumanImgUrl,
                        ResultImgUrl = entity.ResultImgUrl
                    })
                    .ToArray();

                _memoryCache.Set(cacheKey, updatedHistory);

                _logger.LogDebug("Updated memory cache for user {UserId}", entity.AccountId);
            }
            else
            {
                // 4. Если кеша нет - просто инвалидируем (при следующем GetHistory кеш заполнится)
                _memoryCache.Remove(cacheKey);
                _logger.LogDebug("Invalidated memory cache for user {UserId}", entity.AccountId);
            }
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

            if (_memoryCache.TryGetValue(cacheKey, out FittingHistoryResponse[] cachedData))
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
