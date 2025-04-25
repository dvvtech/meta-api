using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services.Interfaces
{
    public interface IVirtualFitService
    {
        Task Delete(int fittingResultId, int userId);

        Task<Result<FittingHistoryResponse[]>> GetHistory(int userId);

        Task<Result<(string ResultImageUrl, int RemainingUsage)>> TryOnClothesAsync(FittingData fittingData);
    }
}
