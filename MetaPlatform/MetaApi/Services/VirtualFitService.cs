using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Interfaces.Services;
using MetaApi.Services.Interfaces;


namespace MetaApi.Services
{    
    public partial class VirtualFitService : IVirtualFitService
    {
        private readonly IReplicateClientService _replicateClientService;
        private readonly IFittingHistoryRepository _fittingHistoryRepository;
        private readonly ITryOnLimitService _tryOnLimitService;        
        private readonly IFileService _fileService;                
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualFitService(IReplicateClientService replicateClientService,
                                 IFittingHistoryRepository fittingHistoryRepository,
                                 ITryOnLimitService tryOnLimitService,                                 
                                 IFileService fileService,                                         
                                 ILogger<VirtualFitService> logger)
        {
            _replicateClientService = replicateClientService;
            _fittingHistoryRepository = fittingHistoryRepository;
            _tryOnLimitService = tryOnLimitService;            
            _fileService = fileService;                    
            _logger = logger;
        }
    }
}
