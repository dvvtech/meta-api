using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services.Interfaces
{
    public interface IReplicateVirtualHairApiClient
    {
        Task<Result<string>> ProcessPredictionAsync(HairTryOnData request);
    }
}
