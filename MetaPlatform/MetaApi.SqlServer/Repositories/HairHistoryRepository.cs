using MetaApi.Core.Domain.Hair;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities.VirtualHair;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{
    public class HairHistoryRepository : IHairHistoryRepository
    {
        private readonly MetaDbContext _dbContext;

        public HairHistoryRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HairHistory[]> GetHistoryAsync(int userId)
        {
            return await _dbContext.HairHistory.AsNoTracking()
                                               .Where(result => result.AccountId == userId && !result.IsDeleted)
                                               .Select(item => HairHistory.Create(item.Id,
                                                                                    item.AccountId,
                                                                                    item.HairImgUrl,
                                                                                    item.FaceImgUrl,
                                                                                    item.ResultImgUrl))
                                               .ToArrayAsync();
        }

        public async Task<DateTime> GetDateOfLastFittingAsync(int userId)
        {
            var lastFitting = await _dbContext.HairHistory
                .AsNoTracking()
                .Where(result => result.AccountId == userId && !result.IsDeleted)
                .OrderByDescending(result => result.CreatedUtcDate) // Сортируем по дате в обратном порядке
                .FirstOrDefaultAsync(); // Берем первую запись (самую новую)

            if (lastFitting != null)
                return lastFitting.CreatedUtcDate;
            else
                return DateTime.MinValue;
        }

        public async Task<int> AddToHistoryAsync(HairHistory entity)
        {
            var newHairHistory = new HairHistoryEntity
            { 
                AccountId = entity.AccountId,
                HairImgUrl = entity.HairImg,
                FaceImgUrl = entity.FaceImg,
                ResultImgUrl = entity.ResultImgUrl,
                CreatedUtcDate = DateTime.UtcNow
            };

            _dbContext.HairHistory.Add(newHairHistory);
            await _dbContext.SaveChangesAsync();

            return newHairHistory.Id;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var hairHistory = await _dbContext.HairHistory
                .FirstOrDefaultAsync(x => x.Id == id && x.AccountId == userId);

            if (hairHistory != null)
            {
                hairHistory.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }                
    }
}
