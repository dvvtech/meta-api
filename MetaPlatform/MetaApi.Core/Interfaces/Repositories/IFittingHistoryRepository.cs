using MetaApi.Core.Domain.FittingHistory;

namespace MetaApi.Core.Interfaces.Repositories
{
    public interface IFittingHistoryRepository
    {
        Task<FittingHistory[]> GetHistoryAsync(int userId);
        Task<int> AddToHistoryAsync(FittingHistory entity);
        Task DeleteAsync(int fittingResultId, int userId);
    }
}
