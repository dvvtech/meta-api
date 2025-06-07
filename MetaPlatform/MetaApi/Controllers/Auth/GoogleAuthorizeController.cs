using MetaApi.Models.Auth;
using MetaApi.Services.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/google-auth")]
    [ApiController]
    public class GoogleAuthorizeController : ControllerBase
    {
        private readonly GoogleAuthService _authService;
        private readonly ILogger<GoogleAuthorizeController> _logger;

        public GoogleAuthorizeController(GoogleAuthService googleAuthService,
                                         ILogger<GoogleAuthorizeController> logger)
        {
            _authService = googleAuthService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            _logger.LogInformation("google authorize");

            var authUrl = _authService.GenerateAuthUrl();            
            return Ok(authUrl);            
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Google auth: Code is required");
                }

                TokenResponse tokenResponse = await _authService.HandleCallback(code);

                //Перенаправляем пользователя на фронтенд
                return Redirect($"https://virtual-fit.one?" +
                                $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                                $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Google auth exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
