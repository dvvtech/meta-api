using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services.Interfaces
{
    public interface IReplicateClientService
    {
        Task<Result<string>> ProcessPredictionAsync(FittingData request);
    }
}
