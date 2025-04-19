using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{
    public interface IFittingHistoryRepository
    {
        Task<FittingResultEntity[]> GetHistoryAsync(int userId);
        Task AddToHistoryAsync(FittingResultEntity entity);
        Task DeleteAsync(int fittingResultId, int userId);
    }

    public class FittingHistoryRepository : IFittingHistoryRepository
    {
        private readonly MetaDbContext _dbContext;

        public FittingHistoryRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FittingResultEntity[]> GetHistoryAsync(int userId)
        {
            return await _dbContext.FittingResult
                .Where(result => result.AccountId == userId && !result.IsDeleted)                
                .ToArrayAsync();
        }

        public async Task AddToHistoryAsync(FittingResultEntity entity)
        {
            _dbContext.FittingResult.Add(entity);
            await _dbContext.SaveChangesAsync();
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
