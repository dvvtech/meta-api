using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services.Interfaces
{
    public interface IVirtualHairStyleService
    {
        Task<Result<string>> TryOnAsync(HairTryOnData hairTryOnData);

        //Task Delete(int fittingResultId, int userId);

        //Task<Result<FittingHistory[]>> GetHistory(int userId);

        //Task<Result<FittingHistory[]>> GetExamples(string host);
    }
}
