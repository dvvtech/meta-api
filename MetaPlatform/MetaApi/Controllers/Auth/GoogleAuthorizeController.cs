using MetaApi.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/google-auth")]
    [ApiController]
    public class GoogleAuthorizeController : ControllerBase
    {
        private GoogleAuthService _authService;

        public GoogleAuthorizeController(GoogleAuthService googleAuthService)
        {
            _authService = googleAuthService;
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            var authUrl = _authService.GenerateAuthUrl();            
            return Ok(authUrl);            
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var tokenResponse = await _authService.HandleCallback(code);            
            return Redirect($"https://localhost:7105/connect/?accessToken={tokenResponse.AccessToken}&refreshToken={tokenResponse.RefreshToken}");
            //Перенаправляем пользователя на фронтенд
            //return Redirect($"https://virtual-fit.one?accessToken={tokenResponse.AccessToken}&refreshToken={tokenResponse.RefreshToken}");
        }
    }
}
