using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Interfaces.Services;
using MetaApi.Services.Interfaces;

namespace MetaApi.Services
{
    public partial class VirtualHairStyleService : IVirtualHairStyleService
    {
        private readonly IReplicateVirtualHairApiClient _replicateClientService;
        private readonly IHairHistoryRepository _hairHistoryRepository;
        private readonly ITryOnLimitService _tryOnLimitService;
        private readonly IFileService _fileService;
        private readonly ILogger<VirtualHairStyleService> _logger;

        public VirtualHairStyleService(
            IReplicateVirtualHairApiClient replicateClientService,
            IHairHistoryRepository hairHistoryRepository,
            ITryOnLimitService tryOnLimitService,
            IFileService fileService,
            ILogger<VirtualHairStyleService> logger)
        {
            _replicateClientService = replicateClientService;
            _hairHistoryRepository = hairHistoryRepository;
            _tryOnLimitService = tryOnLimitService;
            _fileService = fileService;
            _logger = logger;
        }

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
