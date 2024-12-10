using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistoryResponse[]>> GetHistory(string promocode)
        {
            PromocodeEntity? promocodeEntity = await _metaDbContext.Promocode.FirstOrDefaultAsync(p => p.Promocode == promocode);
            if (promocodeEntity == null)
            {
                return Result<FittingHistoryResponse[]>.Failure(VirtualFitError.NotValidPromocodeError());
            }

            FittingHistoryResponse[] fittingResults = await _metaDbContext.FittingResult
                                                                  .Where(result => result.PromocodeId == promocodeEntity.Id)
                                                                  .Select(s => new FittingHistoryResponse
                                                                  {
                                                                      GarmentImgUrl = s.GarmentImgUrl,
                                                                      HumanImgUrl = s.HumanImgUrl,
                                                                      ResultImgUrl = s.ResultImgUrl,
                                                                  })
                                                                  .ToArrayAsync();
            return Result<FittingHistoryResponse[]>.Success(fittingResults);
        }
    }
}
