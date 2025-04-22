using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
using MetaApi.Models.VirtualFit;
using MetaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/try-on-limit")]
    [ApiController]
    public class TryOnLimitController : ControllerBase
    {
        private readonly ITryOnLimitService _tryOnLimitService;
        private readonly ILogger<TryOnLimitController> _logger;

        public TryOnLimitController(ITryOnLimitService tryOnLimitService, 
                                    ILogger<TryOnLimitController> logger)
        {
            _tryOnLimitService = tryOnLimitService;
            _logger = logger;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<LimitResponse>> GetCurrentLimit()
        {
            _logger.LogInformation("get limit");

            Result<int> userIdResult = this.GetCurrentUserId();
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

            int remainingTries = await _tryOnLimitService.GetRemainingTriesAsync(userIdResult.Value);

            return new LimitResponse { RemainingTries = remainingTries };
        }
    }
}
