using MetaApi.Core.OperationResults.Base;
using MetaApi.Extensions;
using MetaApi.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthorizeController> _logger;

        public AuthorizeController(AuthService authService, ILogger<AuthorizeController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("test")]
        public async Task<ActionResult> Test()
        {
            _logger.LogInformation("12345");
            return Ok("123");
        }

        [HttpPost("logout"), Authorize]
        public async Task<ActionResult> Logout()
        {
            Result<string> accountExternalIdResult = this.GetCurrentUserId();
            if (accountExternalIdResult.IsFailure)
            {
                return BadRequest(accountExternalIdResult.Error);
            }

            Result response = await _authService.LogoutAsync(accountExternalIdResult.Value);
            /*if (!response.IsSuccess)
            {
                return BadRequest(response.Error);
            }*/

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponse>> RefreshToken(string refreshToken)
        {
            Result<MetaApi.Models.Auth.TokenResponse> tokenResult = await _authService.RefreshTokenAsync(refreshToken);
            if (tokenResult.IsFailure)
            {
                return BadRequest("Failed to refresh token");
            }

            return Ok(tokenResult.Value);
        }
    }
}
