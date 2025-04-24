using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{    
    public class FittingHistoryRepository : IFittingHistoryRepository
    {
        private readonly MetaDbContext _dbContext;

        public FittingHistoryRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FittingHistory[]> GetHistoryAsync(int userId)
        {
            return await _dbContext.FittingResult.AsNoTracking()
                                                 .Where(result => result.AccountId == userId && !result.IsDeleted)
                                                 .Select(item => FittingHistory.Create(item.Id,
                                                                                       item.AccountId,
                                                                                       item.GarmentImgUrl,
                                                                                       item.HumanImgUrl,
                                                                                       item.ResultImgUrl))
                                                 .ToArrayAsync();                                                             
        }

        public async Task<int> AddToHistoryAsync(FittingHistory item)
        {
            var newFittingHistory = new FittingResultEntity
            {
               AccountId = item.AccountId,
               GarmentImgUrl = item.GarmentImgUrl,
               HumanImgUrl = item.HumanImgUrl,
               ResultImgUrl = item.ResultImgUrl,
               CreatedUtcDate = DateTime.UtcNow
            };

            _dbContext.FittingResult.Add(newFittingHistory);
            await _dbContext.SaveChangesAsync();

            return newFittingHistory.Id;
        }

        public async Task DeleteAsync(int fittingResultId, int userId)
        {
            var fittingResult = await _dbContext.FittingResult
                .FirstOrDefaultAsync(x => x.Id == fittingResultId && x.AccountId == userId);

            if (fittingResult != null)
            {
                fittingResult.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
