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
            var url = _yandexAuthService.GenerateAuthUrl();            
            return Ok(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            _logger.LogInformation($"YandexAuth callback code: {code}");

            var tokenResponse = await _yandexAuthService.HandleCallback(code);
            if (tokenResponse == null)
            {
                return BadRequest("Failed to authenticate with Yandex");
            }

            //Перенаправляем пользователя на фронтенд
            return Redirect($"https://virtual-fit.one?accessToken={tokenResponse.AccessToken}&refreshToken={tokenResponse.RefreshToken}");
        }
    }
}
