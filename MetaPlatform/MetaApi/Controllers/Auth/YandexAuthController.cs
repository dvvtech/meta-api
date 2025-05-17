using MetaApi.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers.Auth
{
    [Route("api/yandex-auth")]
    [ApiController]
    public class YandexAuthController : ControllerBase
    {
        private readonly YandexAuthService _yandexAuthService;

        public YandexAuthController(YandexAuthService yandexAuthService)
        {
            _yandexAuthService = yandexAuthService;
        }

        [HttpGet("auth-url")]
        public IActionResult GetAuthUrl()
        {
            var url = _yandexAuthService.GenerateAuthUrl();
            return Ok(new { Url = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var tokenResponse = await _yandexAuthService.HandleCallback(code);
            if (tokenResponse == null)
            {
                return BadRequest("Failed to authenticate with Yandex");
            }

            return Ok(tokenResponse);
        }
    }
}
