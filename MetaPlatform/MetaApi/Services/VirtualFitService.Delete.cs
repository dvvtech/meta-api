using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task Delete(FittingDeleteRequest request, int userId)
        {
            await _fittingHistoryRepository.DeleteAsync(request.FittingResultId, userId);     
        }
    }
}
