using MetaApi.SqlServer.Repositories;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        private readonly ReplicateClientService _replicateClientService;
        private readonly IFittingHistoryRepository _fittingHistoryRepository;
        private readonly TryOnLimitService _tryOnLimitService;        
        private readonly FileService _fileService;                
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualFitService(ReplicateClientService replicateClientService,
                                 IFittingHistoryRepository fittingHistoryCache,
                                 TryOnLimitService tryOnLimitService,                                 
                                 FileService fileService,                                         
                                 ILogger<VirtualFitService> logger)
        {
            _replicateClientService = replicateClientService;
            _fittingHistoryRepository = fittingHistoryCache;
            _tryOnLimitService = tryOnLimitService;            
            _fileService = fileService;                    
            _logger = logger;
        }
    }
}
