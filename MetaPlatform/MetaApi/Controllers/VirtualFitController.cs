using Microsoft.AspNetCore.Mvc;
using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using MetaApi.Core.OperationResults.Base;
using Microsoft.AspNetCore.Authorization;
using MetaApi.Extensions;
using Microsoft.Extensions.Logging;

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

        /*[HttpGet("clothing-collection")]
        public ActionResult<ClothingCollection> GetClothingCollection()
        {
            return Ok(_virtualFitService.GetClothingCollection(Request.Host.Value));
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
        }*/

        /// <summary>
        /// Примерка одежды
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("try-on"), Authorize]                
        public async Task<ActionResult<FittingResultResponse>> TryOnClothes(FittingRequest request)
        {
            _logger.LogInformation("Start try-on");
            
            Result<int> userIdResult = this.GetCurrentUserId();

            Result<FittingResultResponse> resultFit = await _virtualFitService.TryOnClothesAsync(request, Request.Host.Value, userIdResult.Value);                      
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

        [HttpDelete("history"), Authorize]        
        public async Task<ActionResult> Delete(FittingDeleteRequest request)
        {
            Result<int> userIdResult = this.GetCurrentUserId();
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

            await _virtualFitService.Delete(request, userIdResult.Value);
            return Ok();
        }

        [HttpPost("history"), Authorize]        
        public async Task<ActionResult<FittingHistoryResponse[]>> GetHistory()
        {
            _logger.LogInformation("GetHistory1");
            Result<int> userIdResult = this.GetCurrentUserId();
            _logger.LogInformation("accountId: " + userIdResult.Value.ToString());
            if (userIdResult.IsFailure)
            {
                _logger.LogInformation("GetHistory2");
                return BadRequest(userIdResult.Error);
            }

            _logger.LogInformation("GetHistory3");
            Result<FittingHistoryResponse[]> fittingResults = await _virtualFitService.GetHistory(userIdResult.Value);
            if (fittingResults.IsFailure)
            {
                return BadRequest(new { description = fittingResults.Error.Description });
            }
            _logger.LogInformation("GetHistory4");

            return Ok(fittingResults.Value);
        }

        [HttpGet("test1")]
        public IResult Test()
        {
            _logger.LogInformation("123count_images1: ");
            //_virtualFitService.Test();
            return Results.Ok("777");
        }
    }
}
