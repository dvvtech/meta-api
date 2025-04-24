using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
using MetaApi.Models.Auth;
using MetaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthorizeController> _logger;        

        public AuthorizeController(IAuthService authService, ILogger<AuthorizeController> logger)
        {
            _authService = authService;
            _logger = logger;            
        }        

        [HttpPost("logout"), Authorize]
        public async Task<ActionResult> Logout()
        {
            Result<int> accountExternalIdResult = this.GetCurrentUserId();
            /*if (accountExternalIdResult.IsFailure)
            {
                return BadRequest(accountExternalIdResult.Error);
            }*/

            Result response = await _authService.LogoutAsync(accountExternalIdResult.Value);
            /*if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }*/

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<Models.Auth.TokenResponse>> RefreshToken(RefreshTokenRequest request)
        {
            Result<MetaApi.Models.Auth.TokenResponse> tokenResult = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (tokenResult.IsFailure)
            {
                return BadRequest("Failed to refresh token");
            }

            return Ok(tokenResult.Value);
        }

        [HttpPost("test1")]
        public async Task<ActionResult> Test()
        {
            _logger.LogInformation("555");
            return Ok("555");
        }
    }
}
