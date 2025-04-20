using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistoryResponse[]>> GetHistory(int userId)
        {
            FittingResultEntity[] fittingResults = await _fittingHistoryRepository.GetHistoryAsync(userId);
            var fittingHistories = fittingResults.Select(s => new FittingHistoryResponse
            {
                Id = s.Id,
                GarmentImgUrl = s.GarmentImgUrl,
                HumanImgUrl = s.HumanImgUrl,
                ResultImgUrl = s.ResultImgUrl,
            }).ToArray();

            return Result<FittingHistoryResponse[]>.Success(fittingHistories);

        }
    }
}
