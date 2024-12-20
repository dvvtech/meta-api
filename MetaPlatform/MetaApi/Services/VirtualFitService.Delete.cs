using MetaApi.Consts;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task Delete(FittingDeleteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Promocode) ||
                request.Promocode.Length > FittingConstants.PROMOCODE_MAX_LENGTH)
            {
                return;
            }

            PromocodeEntity? promocode = await _metaDbContext.Promocode.FirstOrDefaultAsync(p => p.Promocode == request.Promocode);
            if (promocode == null)
            {
                return;
            }
            if (promocode.RemainingUsage <= 0)
            {
                return;
            }

            FittingResultEntity? fittingResult = await _metaDbContext.FittingResult.FirstOrDefaultAsync(x => x.Id == request.FittingResultId);
            if (fittingResult != null && fittingResult.PromocodeId == promocode.Id)
            {
                fittingResult.IsDeleted = true;
                await _metaDbContext.SaveChangesAsync();
            }            
        }
    }
}
