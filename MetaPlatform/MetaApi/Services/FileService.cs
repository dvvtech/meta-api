namespace MetaApi.Services
{
    public partial class FileService
    {
        private readonly FileCrcHostedService _fileCrcService;
        private readonly ImageService _imageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;

        public FileService(FileCrcHostedService fileCrcService,
                           ImageService imageService,
                           IHttpClientFactory httpClientFactory,
                           IWebHostEnvironment env,
                           ILogger<FileService> logger)
        {
            _fileCrcService = fileCrcService;
            _imageService = imageService;
            _httpClientFactory = httpClientFactory;
            _env = env;
            _logger = logger;
        }

        public int GetCount()
        {
            return _fileCrcService.FileCrcDictionary.Count;
        } 
    }
}
