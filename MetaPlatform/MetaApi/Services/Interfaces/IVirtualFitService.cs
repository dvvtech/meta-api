using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services.Interfaces
{
    public interface IVirtualFitService
    {
        Task Delete(int fittingResultId, int userId);

        Task<Result<FittingHistory[]>> GetHistory(int userId);

        Task<Result<(string ResultImageUrl, int RemainingUsage)>> TryOnClothesAsync(FittingData fittingData);        
    }
}
