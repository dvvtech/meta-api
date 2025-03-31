using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task Delete(FittingDeleteRequest request, int userId)
        {                        
            FittingResultEntity? fittingResult = await _metaDbContext.FittingResult.FirstOrDefaultAsync(x => x.Id == request.FittingResultId);
            if (fittingResult != null && fittingResult.AccountId == userId)
            {
                fittingResult.IsDeleted = true;
                await _metaDbContext.SaveChangesAsync();
            }            
        }
    }
}
