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
                                       hairImgUrl: GenerateFileUrl("70bb7586-bfdf-4c25-be91-ad16e99b2bb6_0.936_t.png", FileType.HairExamples, host),
                                       faceImgUrl: GenerateFileUrl("13fc6814-7952-4b17-aa2b-f3944820def1_t.png", FileType.HairExamples, host),
                                       resultImgUrl: GenerateFileUrl("5428fb07-8a06-4236-8159-dd127e87f36f_t.jpeg", FileType.HairExamples, host)),
                    HairHistory.Create(accountId: 1,
                                       hairImgUrl: GenerateFileUrl("fa3f0c40-11ed-401f-b30f-8137d3fd8ded_0.846_t.png", FileType.HairExamples, host),
                                       faceImgUrl: GenerateFileUrl("48ba878b-cf80-4491-9197-26b52012cb7d_0.605_t.png", FileType.HairExamples, host),
                                       resultImgUrl: GenerateFileUrl("fbaa162a-9fbb-414e-8e7a-0cbcb29108d1_0.625_t.jpeg", FileType.HairExamples, host)),
                    HairHistory.Create(accountId: 1,
                                       hairImgUrl: GenerateFileUrl("0450ed64-7ed4-4d22-8c00-968d1fb44fc6_1.004_t.png", FileType.HairExamples, host),
                                       faceImgUrl: GenerateFileUrl("554356e7-2239-4c13-8ce1-276b76bf6a72_1.136_t.png", FileType.HairExamples, host),
                                       resultImgUrl: GenerateFileUrl("7b057d3e-d443-4d0d-8fce-5fc879e81ec2_1.133_v.jpeg", FileType.HairExamples, host))
            };            

            return Result<HairHistory[]>.Success(fittingExamples);
        }

        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
