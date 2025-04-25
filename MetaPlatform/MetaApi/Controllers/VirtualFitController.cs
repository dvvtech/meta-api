using Microsoft.AspNetCore.Mvc;
using MetaApi.Models.VirtualFit;
using MetaApi.Core.OperationResults.Base;
using Microsoft.AspNetCore.Authorization;
using MetaApi.Extensions;
using MetaApi.Services.Interfaces;
using MetaApi.Core.Domain.FittingHistory;

namespace MetaApi.Controllers
{
    /// <summary>
    /// Сервис примерки одежды. Используется модель https://replicate.com/cuuupid/idm-vton
    /// </summary>
    [Route("api/virtual-fit")]
    [ApiController]
    public class VirtualFitController : ControllerBase
    {
        private readonly IVirtualFitService _virtualFitService;        
        private readonly ILogger<VirtualFitController> _logger;        

        public VirtualFitController(IVirtualFitService virtualFitService,                                    
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

            var fittingData = new FittingData
            {
                GarmImg = request.GarmImg,
                HumanImg = request.HumanImg,
                Category = request.Category,
                AccountId = userIdResult.Value,
                Host = Request.Host.Value
            };

            Result<(string ResultImageUrl, int RemainingUsage)> resultFit = await _virtualFitService.TryOnClothesAsync(fittingData);                      
            if (resultFit.IsFailure)
            {
                _logger.LogError($"try-on failed: {resultFit.Error.Description}");

                if (int.TryParse(resultFit.Error.Code, out int httpStatusCode))
                {
                    return StatusCode(httpStatusCode, new { description = resultFit.Error.Description });
                }
                else
                {                    
                    return BadRequest(new { description = resultFit.Error.Description });
                }
            }

            return Ok(new FittingResultResponse { Url = resultFit.Value.ResultImageUrl, RemainingUsage = resultFit.Value.RemainingUsage });            
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
