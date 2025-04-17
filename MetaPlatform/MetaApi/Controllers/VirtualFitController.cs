using Microsoft.AspNetCore.Mvc;
using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using MetaApi.Core.OperationResults.Base;
using Microsoft.AspNetCore.Authorization;
using MetaApi.Extensions;

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
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

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

            _logger.LogInformation("Success try-on");
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
            _logger.LogInformation("GetHistory");

            Result<int> userIdResult = this.GetCurrentUserId();            
            if (userIdResult.IsFailure)
            {                
                return BadRequest(userIdResult.Error);
            }
            
            Result<FittingHistoryResponse[]> fittingResults = await _virtualFitService.GetHistory(userIdResult.Value);
            if (fittingResults.IsFailure)
            {
                return BadRequest(new { description = fittingResults.Error.Description });
            }

            return Ok(fittingResults.Value);
        }

        [HttpGet("test3")]
        public IResult Test()
        {
            _logger.LogInformation("123count_images1: ");
            //_virtualFitService.Test();
            return Results.Ok("777");
        }
    }
}
