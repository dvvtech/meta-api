using MetaApi.SqlServer.Context;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        private readonly MetaDbContext _metaDbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FileService _fileService;                
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualFitService(MetaDbContext metaContext,
                                 IHttpClientFactory httpClientFactory,
                                 FileService fileService,                                         
                                 ILogger<VirtualFitService> logger)
        {
            _metaDbContext = metaContext;
            _httpClientFactory = httpClientFactory;
            _fileService = fileService;                    
            _logger = logger;
        }
    }
}
