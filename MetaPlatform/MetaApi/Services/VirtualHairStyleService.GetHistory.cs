using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services
{
    public partial class VirtualHairStyleService
    {
        public async Task<Result<HairHistory[]>> GetHistory(int userId)
        {
            HairHistory[] hairResults = await _hairHistoryRepository.GetHistoryAsync(userId);
            return Result<HairHistory[]>.Success(hairResults);
        }
    }
}
