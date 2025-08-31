using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
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
                ColorImg = request.ColorImg,
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
    }
}
