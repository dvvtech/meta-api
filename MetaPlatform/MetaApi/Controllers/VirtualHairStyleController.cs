using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
using MetaApi.Models.VirtualFit;
using MetaApi.Models.VirtualHair;
using MetaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/virtual-hair")]
    [ApiController]
    public class VirtualHairStyleController : ControllerBase
    {
        private readonly IVirtualHairStyleService _virtualHairStyleService;
        private readonly ILogger<VirtualHairStyleController> _logger;

        public VirtualHairStyleController(
            IVirtualHairStyleService virtualFitService,
            ILogger<VirtualHairStyleController> logger)
        {
            _virtualHairStyleService = virtualFitService;
            _logger = logger;
        }
        /// <summary>
        /// Примерка одежды
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPost("try-on"), Authorize]
        public async Task<ActionResult<HairResultResponse>> TryOnHair(HairTryOnRequest request)
        {
            _logger.LogInformation("Start try-on hair style");

            Result<int> userIdResult = this.GetCurrentUserId();
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

            var hairTryOnData = new HairTryOnData
            {
                HairImg = request.HairImg,
                FaceImg = request.FaceImg,                
                AccountId = userIdResult.Value,
                Host = Request.Host.Value
            };

            Result<string> resultHairTryOn = await _virtualHairStyleService.TryOnAsync(hairTryOnData);
            if (resultHairTryOn.IsFailure)
            {
                _logger.LogError($"try-on failed: {resultHairTryOn.Error.Description}");

                if (int.TryParse(resultHairTryOn.Error.Code, out int httpStatusCode))
                {
                    return StatusCode(httpStatusCode, new { description = resultHairTryOn.Error.Description });
                }
                else
                {
                    return BadRequest(new { description = resultHairTryOn.Error.Description });
                }
            }

            return Ok(new HairResultResponse { Url = resultHairTryOn.Value });
        }

        [HttpDelete("history"), Authorize]
        public async Task<ActionResult> Delete(HairDeleteRequest request)
        {
            Result<int> userIdResult = this.GetCurrentUserId();
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

            await _virtualHairStyleService.Delete(request.HairResultId, userIdResult.Value);
            return Ok();
        }

        [HttpPost("history"), Authorize]
        public async Task<ActionResult<HairHistoryResponse[]>> GetHistory()
        {
            _logger.LogInformation("GetHistory");

            Result<int> userIdResult = this.GetCurrentUserId();
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

            Result<HairHistory[]> fittingResults = await _virtualHairStyleService.GetHistory(userIdResult.Value);
            if (fittingResults.IsFailure)
            {
                return BadRequest(new { description = fittingResults.Error.Description });
            }

            var fittingHistories = fittingResults.Value.Select(s => new HairHistoryResponse
            {
                Id = s.Id,
                FaceImgUrl = s.FaceImg,
                HairImgUrl = s.HairImg,
                ResultImgUrl = s.ResultImgUrl,
            }).ToArray();

            return Ok(fittingHistories);
        }

        /// <summary>
        /// Примеры примерок для незарегестрированных пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet("examples")]
        public async Task<ActionResult<FittingHistoryResponse[]>> GetExamples()
        {
            _logger.LogInformation("GetExamples");

            Result<HairHistory[]> fittingResults = await _virtualHairStyleService.GetExamples(Request.Host.Value);
            if (fittingResults.IsFailure)
            {
                return BadRequest(new { description = fittingResults.Error.Description });
            }

            var fittingExamples = fittingResults.Value.Select(s => new HairHistoryResponse
            {
                Id = s.Id,
                FaceImgUrl = s.FaceImg,
                HairImgUrl = s.HairImg,
                ResultImgUrl = s.ResultImgUrl,
            }).ToArray();

            return Ok(fittingExamples);
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
