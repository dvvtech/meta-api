using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistoryResponse[]>> GetHistory(int userId)
        {
            FittingHistoryResponse[] fittingResults = await _fittingHistoryCache.GetHistory(userId);

            return Result<FittingHistoryResponse[]>.Success(fittingResults);

        }
    }
}
