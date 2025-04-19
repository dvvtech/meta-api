using MetaApi.Services.Cache;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        private readonly FittingHistoryCache _fittingHistoryCache;
        private readonly TryOnLimitService _tryOnLimitService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FileService _fileService;                
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualFitService(FittingHistoryCache fittingHistoryCache,
                                 TryOnLimitService tryOnLimitService,
                                 IHttpClientFactory httpClientFactory,
                                 FileService fileService,                                         
                                 ILogger<VirtualFitService> logger)
        {
            _fittingHistoryCache = fittingHistoryCache;
            _tryOnLimitService = tryOnLimitService;
            _httpClientFactory = httpClientFactory;
            _fileService = fileService;                    
            _logger = logger;
        }
    }
}
