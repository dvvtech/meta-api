using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualHairStyleService
    {
        public async Task<Result<HairHistory[]>> GetHistory(int userId)
        {
            HairHistory[] hairResults = await _hairHistoryRepository.GetHistoryAsync(userId);
            return Result<HairHistory[]>.Success(hairResults);
        }

        /// <summary>
        /// Примеры примерок для незарегестрированных пользователей
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public async Task<Result<HairHistory[]>> GetExamples(string host)
        {

            var fittingExamples = new HairHistory[]
            {
                    HairHistory.Create(accountId: 1,
                                          hairImgImgUrl: GenerateFileUrl("2e39b1d1-dfc0-4c41-a600-e5b72da6220a_1.435_t.png", FileType.Examples, host),
                                          faceImgUrl: GenerateFileUrl("547babf5-f046-4b73-aa2c-a7d4494298d3_1.481_t.png", FileType.Examples, host),
                                          resultImgUrl: GenerateFileUrl("7abd958d-eb5b-4180-a64f-c988fc536f02_t.jpg", FileType.Examples, host)),
                    HairHistory.Create(accountId: 1,
                                          hairImgImgUrl: GenerateFileUrl("de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png", FileType.Examples, host),
                                          faceImgUrl: GenerateFileUrl("3ac9d1c3-4fae-455b-9bef-30e9a2395a7b_2.433_t.png", FileType.Examples, host),
                                          resultImgUrl: GenerateFileUrl("9b9ad00a-47b9-48e8-b960-b73fffc6eabe_t.jpg", FileType.Examples, host)),
                    HairHistory.Create(accountId: 1,
                                          hairImgImgUrl: GenerateFileUrl("22069f77-18f3-4d3a-8988-4f8ce9ee8726_1.016_t.png", FileType.Examples, host),
                                          faceImgUrl: GenerateFileUrl("7def122e-b614-4f12-b596-9a0b6ccc1a6a_1.497_v.png", FileType.Examples, host),
                                          resultImgUrl: GenerateFileUrl("bbd9ea7b-3bdd-41b6-9992-340ae79ea933_t.jpg", FileType.Examples, host))
            };


            return Result<HairHistory[]>.Success(fittingExamples);
        }

        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
