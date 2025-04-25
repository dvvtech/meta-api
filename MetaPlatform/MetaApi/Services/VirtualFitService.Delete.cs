using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task Delete(int fittingResultId, int userId)
        {
            await _fittingHistoryRepository.DeleteAsync(fittingResultId, userId);     
        }
    }
}
