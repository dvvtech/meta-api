using MetaApi.Services.Interfaces;
using MetaApi.SqlServer.Repositories;

namespace MetaApi.Services
{    
    public partial class VirtualFitService : IVirtualFitService
    {
        private readonly IReplicateClientService _replicateClientService;
        private readonly IFittingHistoryRepository _fittingHistoryRepository;
        private readonly ITryOnLimitService _tryOnLimitService;        
        private readonly FileService _fileService;                
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualFitService(IReplicateClientService replicateClientService,
                                 IFittingHistoryRepository fittingHistoryRepository,
                                 ITryOnLimitService tryOnLimitService,                                 
                                 FileService fileService,                                         
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
