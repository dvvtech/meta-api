using MetaApi.Services.Interfaces;

namespace MetaApi.Services
{
    public partial class FileService : IFileService
    {
        private readonly ICrcFileProvider _crcFileProvider;
        private readonly ImageService _imageService;
        private readonly IHttpClientFactory _httpClientFactory;        
        private readonly ILogger<FileService> _logger;
        private readonly string _webRootPath;

        public FileService(ICrcFileProvider fileCrcService,
                           ImageService imageService,
                           IHttpClientFactory httpClientFactory,                           
                           ILogger<FileService> logger,
                           string webRootPath)
        {
            _crcFileProvider = fileCrcService;
            _imageService = imageService;
            _httpClientFactory = httpClientFactory;            
            _logger = logger;
            _webRootPath = webRootPath;
        }

        public int GetCount()
        {
            return _crcFileProvider.FileCrcDictionary.Count;
        } 
    }
}
