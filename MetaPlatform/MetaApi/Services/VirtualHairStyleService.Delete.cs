namespace MetaApi.Services
{
    public partial class VirtualHairStyleService
    {
        public async Task Delete(int fittingResultId, int userId)
        {
            await _hairHistoryRepository.DeleteAsync(fittingResultId, userId);
        }
    }
}
