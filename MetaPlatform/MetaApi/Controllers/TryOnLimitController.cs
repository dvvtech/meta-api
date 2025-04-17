using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/try-on-limit")]
    [ApiController]
    public class TryOnLimitController : ControllerBase
    {
        private readonly TryOnLimitService _tryOnLimitService;
        private readonly ILogger<TryOnLimitController> _logger;

        public TryOnLimitController(TryOnLimitService tryOnLimitService, 
                                    ILogger<TryOnLimitController> logger)
        {
            _tryOnLimitService = tryOnLimitService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<LimitResponse> GetCurrentLimit()
        {
            _logger.LogInformation("get limit");

            Result<int> userIdResult = this.GetCurrentUserId();

            int remainingTries = await _tryOnLimitService.GetRemainingTriesAsync(userIdResult.Value);

            return new LimitResponse { RemainingTries = remainingTries };
        }
    }
}
