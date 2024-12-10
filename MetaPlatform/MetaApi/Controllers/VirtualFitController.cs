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
        private readonly ILogger<VirtualFitController> _logger;        

        public VirtualFitController(VirtualFitService virtualFitService,                                    
                                    ILogger<VirtualFitController> logger)
        {
            _virtualFitService = virtualFitService;                        
            _logger = logger;            
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
                var fileUrl = await _virtualFitService.UploadFileAsync(file, Request);
                return Ok(new { url = fileUrl });                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
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
