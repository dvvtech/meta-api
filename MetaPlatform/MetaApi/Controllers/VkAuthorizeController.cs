using MetaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;


namespace MetaApi.Controllers
{
    [Route("api/vk-authorize")]
    [ApiController]
    public class VkAuthorizeController : ControllerBase
    {
        private readonly ILogger<VkAuthorizeController> _logger;

        private readonly VkAuthService _authService;

        public VkAuthorizeController(ILogger<VkAuthorizeController> logger, VkAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("test")]
        public async Task<IActionResult> Test(string token)
        {
            _logger.LogInformation("test");

            /*bool res = await _authService.ValidateToken(token, "", "");
            if (res)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }*/
            return Ok("return");
        }

        /// <summary>
        /// Возвращаем пользователю урл для авторизации в вк
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Authorize()
        {
            _logger.LogInformation("authorize");

            var authUrl = _authService.GenerateAuthUrl();
            //mabre Redirect(authUrl) and rename Authorize to Login
            return Ok(authUrl);
            //return Redirect(authUrl);

            /*string clientId = "53137675";//from config
            string scope = "email";
            string redirectUrl = "https://a30944-8332.x.d-f.pw/api/vk-authorize/callback";//from config
            string state = "XXXRandomZZZ";

            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            var authUrl = $"https://id.vk.com/authorize?response_type=code&client_id={clientId}&scope={scope}&redirect_uri={redirectUrl}&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256";

            return Ok(authUrl);*/

            //https://oauth.vk.com/authorize. 

            //string baseUrl = "https://api.vkontakte.ru/oauth/authorize";
            //string clientId = "53137675";//from config
            //string redirectUrl = "https://a30944-8332.x.d-f.pw/api/vk-authorize/callback";//"https://mmm.com/vk-authorize/callback";//from config
            //string scope = "email";
            //string response_type = "code";

            //string vkAuthUrl = $"{baseUrl}?client_id={clientId}&scope={scope}&redirect_uri={redirectUrl}&response_type={response_type}";

            //return Ok(vkAuthUrl);
        }


        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code,
                                                  [FromQuery] int expires_in,
                                                  [FromQuery] string device_id,
                                                  [FromQuery] string state,
                                                  [FromQuery] string ext_id,
                                                  [FromQuery] string type)
        {
            _logger.LogInformation("Callback7");
            /*_logger.LogInformation($"callback code: {code} {Environment.NewLine}" +
                                    $"expires_in: {expires_in}  {Environment.NewLine}" +
                                    $"device_id: {device_id}  {Environment.NewLine}" +
                                    $"state: {state}  {Environment.NewLine}" +
                                    $"ext_id: {ext_id}  {Environment.NewLine}" +
                                    $"type: {type}  {Environment.NewLine}");*/

            try
            {                
                await _authService.HandleCallback(code, state, device_id);
                _logger.LogInformation("Callback8");
                //return Ok("Authorization successful");
                //Перенаправляем пользователя на фронтенд
                return Redirect("https://virtual-fit.one?accessToken=123567&refreshToken={tokens.RefreshToken}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            // Теперь у вас есть код авторизации, который можно обменять на токен
            //var accessToken = ExchangeCodeForToken(code);

            //return Ok();
        }

    }

}
