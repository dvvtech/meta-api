using MetaApi.Core.OperationResults.Base;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Services.Interfaces
{
    public interface IReplicateClientService
    {
        Task<Result<string>> ProcessPredictionAsync(FittingRequest request);
    }
}
