using MetaApi.Core.Domain.Hair;

namespace MetaApi.Core.Interfaces.Repositories
{
    public interface IHairHistoryRepository
    {
        Task<HairHistory[]> GetHistoryAsync(int userId);
        Task<int> AddToHistoryAsync(HairHistory entity);
        Task DeleteAsync(int fittingResultId, int userId);

        Task<DateTime> GetDateOfLastFittingAsync(int userId);
    }
}
