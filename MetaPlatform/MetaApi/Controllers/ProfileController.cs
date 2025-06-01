using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Interfaces.Services;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<FittingProfile>> GetProfile()
        {
            Result<int> userIdResult = this.GetCurrentUserId();
            if (userIdResult.IsFailure)
            {
                return BadRequest(userIdResult.Error);
            }

            FittingProfile res = await _profileService.GetProfile(userIdResult.Value);

            return Ok(res);
        }
    }
}
