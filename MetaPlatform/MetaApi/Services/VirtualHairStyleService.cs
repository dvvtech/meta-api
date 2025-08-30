using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Services.Interfaces;

namespace MetaApi.Services
{
    public partial class VirtualHairStyleService : IVirtualHairStyleService
    {
        private readonly IReplicateVirtualHairApiClient _replicateClientService;
        private readonly IFileService _fileService;
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualHairStyleService(IReplicateVirtualHairApiClient replicateClientService)
        {
            _replicateClientService = replicateClientService;
        }

        //public Task Delete(int fittingResultId, int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Result<FittingHistory[]>> GetExamples(string host)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Result<FittingHistory[]>> GetHistory(int userId)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
