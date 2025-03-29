using Microsoft.AspNetCore.Mvc;
using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using MetaApi.Core.OperationResults.Base;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("clothing-collection")]
        public ActionResult<ClothingCollection> GetClothingCollection()
        {
            return Ok(_virtualFitService.GetClothingCollection(Request.Host.Value));
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

        [HttpPost("upload-to-collection")]
        public async Task<ActionResult<string>> UploadToCollection(IFormFile file, string promocode, FileType fileType)
        {            
            //http://localhost:5023/api/virtual-fit/upload-to-collection?promocode=123&fileType=3
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пустой.");
            }
            //todo проверка на админский промокод
            //request.Promcode

            try
            {
                var fileUrl = await _virtualFitService.UploadFileAsync(file, fileType, Request.Host.Value);
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
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
                var fileUrl = await _virtualFitService.UploadFileAsync(file, FileType.Upload, Request.Host.Value);
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
        [Authorize]
        public async Task<ActionResult<FittingResultResponse>> TryOnClothes(FittingRequest request)
        {
            _logger.LogInformation("Start try on");
            /*Result<FittingResultResponse> resultFit = await _virtualFitService.TryOnClothesAsync(request, Request.Host.Value);                      
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
             */

            FittingResultResponse resultFit = new FittingResultResponse
            {
                Url = "https://a33140-9deb.k.d-f.pw/oldv2/result/563117e4-9ca2-4460-9e02-c154587ef137_t.jpg",
                RemainingUsage = 4
            };

            return Ok(resultFit);
        }

        [HttpDelete("history")]
        public async Task<ActionResult> Delete(FittingDeleteRequest request)
        { 
            await _virtualFitService.Delete(request);
            return Ok();
        }

        [HttpPost("history")]
        public async Task<ActionResult<FittingHistoryResponse[]>> GetHistory(HistoryRequest request)
        {
            Result<FittingHistoryResponse[]> fittingResults = await _virtualFitService.GetHistory(request.Promocode);
            if (fittingResults.IsFailure)
            {
                return BadRequest(new { description = fittingResults.Error.Description });
            }

            return Ok(fittingResults.Value);
        }

        [HttpGet("test")]
        public string Test()
        {
            _logger.LogInformation("123count_images1: " + _virtualFitService.GetCount());
            //_virtualFitService.Test();
            return "777";
        }
    }
}
