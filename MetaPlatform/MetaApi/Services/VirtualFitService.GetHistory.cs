using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistory[]>> GetHistory(int userId)
        {
            FittingHistory[] fittingResults = await _fittingHistoryRepository.GetHistoryAsync(userId);            
            return Result<FittingHistory[]>.Success(fittingResults);
        }
    }
}
