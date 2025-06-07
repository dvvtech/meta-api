using MetaApi.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers.Auth
{
    [Route("api/mailru-auth")]
    [ApiController]
    public class MailRuAuthorizeController : ControllerBase
    {
        private readonly MailRuAuthService _authService;
        private readonly ILogger<MailRuAuthorizeController> _logger;

        public MailRuAuthorizeController(MailRuAuthService authService,
                                         ILogger<MailRuAuthorizeController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Возвращаем пользователю URL для авторизации в Mail.ru
        /// </summary>
        [HttpGet]
        public IActionResult Authorize()
        {
            _logger.LogInformation("Mail.ru authorize");

            var authUrl = _authService.GenerateAuthUrl();
            return Ok(authUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code,
                                                  [FromQuery] string state,
                                                  [FromQuery] string error,
                                                  [FromQuery] string error_description)
        {
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest("Mail.ru auth: Code is required");
                    //throw new Exception($"Mail.ru auth error: {error}, description: {error_description}");
                }

                TokenResponse tokenResponse = await _authService.HandleCallback(code, state);
                // Перенаправляем пользователя на фронтенд
                return Redirect($"https://virtual-fit.one?" +
                              $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                              $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mailru auth exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
