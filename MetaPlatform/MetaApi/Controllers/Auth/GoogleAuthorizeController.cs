using MetaApi.Services.Auth;
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
            var authUrl = _authService.GenerateAuthUrl();            
            return Ok(authUrl);            
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {            
            var tokenResponse = await _authService.HandleCallback(code);

            // Устанавливаем куки с токенами
            /*Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true, // Защита от XSS
                Secure = true,    // Только HTTPS
                SameSite = SameSiteMode.Strict, // Защита от CSRF
                Expires = DateTime.UtcNow.AddHours(1) // Время жизни access-токена
            });

            Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Время жизни refresh-токена
            });

            // Редирект на фронтенд
            return Redirect("https://virtual-fit.one");*/

            //Перенаправляем пользователя на фронтенд
            return Redirect($"https://virtual-fit.one?" +
                            $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                            $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");

            //return Redirect($"https://localhost:7105/connect/?accessToken={tokenResponse.AccessToken}&refreshToken={tokenResponse.RefreshToken}");            
        }
    }
}
