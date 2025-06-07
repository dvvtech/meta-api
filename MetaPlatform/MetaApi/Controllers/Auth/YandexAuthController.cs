using MetaApi.Models.Auth;
using MetaApi.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers.Auth
{
    [Route("api/yandex-auth")]
    [ApiController]
    public class YandexAuthController : ControllerBase
    {
        private readonly YandexAuthService _yandexAuthService;
        private readonly ILogger<YandexAuthController> _logger;

        public YandexAuthController(YandexAuthService yandexAuthService,
                                    ILogger<YandexAuthController> logger)
        {
            _yandexAuthService = yandexAuthService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAuthUrl()
        {
            _logger.LogInformation("yandex authorize");

            var authUrl = _yandexAuthService.GenerateAuthUrl();            
            return Ok(authUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("Code is required");
                }

                TokenResponse tokenResponse = await _yandexAuthService.HandleCallback(code);
                if (tokenResponse == null)
                {
                    return BadRequest("Failed to authenticate with Yandex");
                }

                //Перенаправляем пользователя на фронтенд
                return Redirect($"https://virtual-fit.one?" +
                                $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                                $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Yandex auth Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
