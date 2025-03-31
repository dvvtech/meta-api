using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistoryResponse[]>> GetHistory(int userId)
        {            
            FittingHistoryResponse[] fittingResults = await _metaDbContext.FittingResult
                                                                  .Where(result => result.AccountId == userId && !result.IsDeleted)
                                                                  .Select(s => new FittingHistoryResponse
                                                                  {
                                                                      Id = s.Id,
                                                                      GarmentImgUrl = s.GarmentImgUrl,
                                                                      HumanImgUrl = s.HumanImgUrl,
                                                                      ResultImgUrl = s.ResultImgUrl,
                                                                  })
                                                                  .ToArrayAsync();
            
            return Result<FittingHistoryResponse[]>.Success(fittingResults);
        }
    }
}
