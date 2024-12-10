using Microsoft.AspNetCore.Mvc;
using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Controllers
{
    /// <summary>
    /// Сервис примерки одежды. Используется модель https://replicate.com/cuuupid/idm-vton
    /// </summary>
    [Route("api/virtual-fit")]
    [ApiController]
    public class VirtualFitController : ControllerBase
    {
        private readonly VirtualFitService _virtualFitService;
        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;        
        private readonly ILogger<VirtualFitController> _logger;
        private readonly FileCrcHostedService _fileCrcService;

        public VirtualFitController(VirtualFitService virtualFitService,
                                    IWebHostEnvironment env,
                                    FileCrcHostedService fileCrcService,
                                    ILogger<VirtualFitController> logger)
        {
            _virtualFitService = virtualFitService;            
            _env = env;   
            _logger = logger;
            _fileCrcService = fileCrcService;
        }

        [HttpGet("test")]
        public string Test()
        {
            _logger.LogInformation("test345");
            return "1234567";
        }

        [HttpPost("generate-promocode")]
        public async Task<ActionResult<string>> GeneratePromocode(GeneratePromocodeRequest request)
        {
            //todo этот метод должен только админом вызываться

            if (request.UsageLimit <= 0)
            {
                return BadRequest("UsageLimit должен быть больше 0.");
            }

            string promocode = await _virtualFitService.GeneratePromocode(request);

            return Ok(promocode);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> Upload(IFormFile file)
        {            
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пустой.");
            }
            
            try
            {
                // Расчёт CRC для загружаемого файла
                string fileCrc = await CalculateCrcAsync(file);

                // Проверка, существует ли файл с таким же CRC
                if (_fileCrcService.FileCrcDictionary.TryGetValue(fileCrc, out var existingFileName))
                {
                    // Файл уже существует, возвращаем ссылку
                    var existingFileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{existingFileName}";
                    return Ok(new { url = existingFileUrl });
                }

                // Путь для сохранения файла
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");                
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                
                // Уникальное имя файла
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Полный путь для сохранения
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Сохранение файла
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Добавление CRC и имени файла через сервис
                _fileCrcService.AddFileCrc(fileCrc, uniqueFileName);

                // Генерация публичной ссылки
                //todo проверить что это https Request.Scheme 
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        private async Task<string> CalculateCrcAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var crc32 = new Crc32();
            var hash = await crc32.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Примерка одежды
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("try-on")]        
        public async Task<ActionResult<string>> TryOnRequest(FittingRequest request)
        {            
            Result<FittingResultResponse> resultFit = await _virtualFitService.TryOnClothesFakeAsync(request);
            //Result<FittingResultResponse> resultFit = await _virtualFitService.TryOnClothesAsync(request);
            if (resultFit.IsFailure)
            {
                if (int.TryParse(resultFit.Error.Code, out int httpStatusCode))
                {
                    return StatusCode(httpStatusCode, new { description = resultFit.Error.Description });
                }
                else
                {                    
                    return BadRequest(new { description = resultFit.Error.Description });
                }
            }

            return Ok(resultFit.Value);            
        }
        [HttpPost("history")]
        public async Task<ActionResult<FittingHistoryResponse[]>> GetHistory(string promocode)
        {
            Result<FittingHistoryResponse[]> fittingResults = await _virtualFitService.GetHistory(promocode);
            if (fittingResults.IsFailure)
            {
                return BadRequest(new { description = fittingResults.Error.Description });
            }

            return Ok(fittingResults.Value);
        }
    }
}
