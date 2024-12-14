using MetaApi.SqlServer.Context;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        private readonly MetaDbContext _metaDbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        private readonly FileCrcHostedService _fileCrcService;
        private readonly ILogger<VirtualFitService> _logger;

        public VirtualFitService(MetaDbContext metaContext,
                                 IHttpClientFactory httpClientFactory,
                                 IWebHostEnvironment env,
                                 FileCrcHostedService fileCrcService,
                                 ILogger<VirtualFitService> logger)
        {
            _metaDbContext = metaContext;
            _httpClientFactory = httpClientFactory;
            _env = env;
            _fileCrcService = fileCrcService;
            _logger = logger;
        }
    }
}
