using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingHistory[]>> GetHistory(int userId)
        {
            FittingHistory[] fittingResults = await _fittingHistoryRepository.GetHistoryAsync(userId);            
            return Result<FittingHistory[]>.Success(fittingResults);
        }

        /// <summary>
        /// Примеры примерок для незарегестрированных пользователей
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public async Task<Result<FittingHistory[]>> GetExamples(string host)
        {

            var fittingExamples = new FittingHistory[]
            {
                    FittingHistory.Create(accountId: 1,
                                          garmentImgUrl: GenerateFileUrl("2e39b1d1-dfc0-4c41-a600-e5b72da6220a_1.435_t.png", FileType.Examples, host),
                                          humanImgUrl: GenerateFileUrl("547babf5-f046-4b73-aa2c-a7d4494298d3_1.481_t.png", FileType.Examples, host),
                                          resultImgUrl: GenerateFileUrl("7abd958d-eb5b-4180-a64f-c988fc536f02_t.jpg", FileType.Examples, host)),
                    FittingHistory.Create(accountId: 1,
                                          garmentImgUrl: GenerateFileUrl("de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png", FileType.Examples, host),
                                          humanImgUrl: GenerateFileUrl("3ac9d1c3-4fae-455b-9bef-30e9a2395a7b_2.433_t.png", FileType.Examples, host),
                                          resultImgUrl: GenerateFileUrl("9b9ad00a-47b9-48e8-b960-b73fffc6eabe_t.jpg", FileType.Examples, host))
            };

            
            return Result<FittingHistory[]>.Success(fittingExamples);
        }

        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {            
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }

    }
}
