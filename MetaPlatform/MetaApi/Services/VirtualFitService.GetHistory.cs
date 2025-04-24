using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistoryResponse[]>> GetHistory(int userId)
        {
            FittingHistory[] fittingResults = await _fittingHistoryRepository.GetHistoryAsync(userId);
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
