using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers.Auth
{
    [Route("api/gazpromid-auth")]
    [ApiController]
    public class GazpromIdAuthorizeController : ControllerBase
    {
        private readonly GazpromIdAuthService _authService;
        private readonly ILogger<GazpromIdAuthorizeController> _logger;

        public GazpromIdAuthorizeController(GazpromIdAuthService authService,
                                          ILogger<GazpromIdAuthorizeController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            var authUrl = _authService.GenerateAuthUrl();
            return Ok(authUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Gazprom ID auth: Code is required");

                var tokenResponse = await _authService.HandleCallback(code, state);
                return Redirect($"https://virtual-fit.one?" +
                              $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                              $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Gazprom ID auth exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
