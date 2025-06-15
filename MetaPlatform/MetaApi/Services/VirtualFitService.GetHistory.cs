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

        public async Task<Result<FittingHistory[]>> GetExamples(int userId, string host)
        {

            var fittingExamples = new FittingHistory[]
            {
                    FittingHistory.Create(accountId: 1,
                                          garmentImgUrl: host + @"examples\2e39b1d1-dfc0-4c41-a600-e5b72da6220a_1.435_t",
                                          humanImgUrl: host + @"examples\547babf5-f046-4b73-aa2c-a7d4494298d3_1.481_t",
                                          resultImgUrl: host + @"examples\547babf5-f046-4b73-aa2c-a7d4494298d3_1.481_t")
            };

            
            return Result<FittingHistory[]>.Success(fittingExamples);
        }

    }
}
