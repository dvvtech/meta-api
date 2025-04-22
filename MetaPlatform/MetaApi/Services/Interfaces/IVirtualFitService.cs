using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services.Interfaces
{
    public interface IVirtualFitService
    {
        Task Delete(FittingDeleteRequest request, int userId);

        Task<Result<FittingHistoryResponse[]>> GetHistory(int userId);

        Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request, string host, int userId);
    }
}
