using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services.Interfaces
{
    public interface IReplicateVirtualFitApiClient
    {
        Task<Result<string>> ProcessPredictionAsync(FittingData request);
    }
}
