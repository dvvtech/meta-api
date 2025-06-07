using MetaApi.Models.Auth;
using MetaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/vk-auth")]
    [ApiController]
    public class VkAuthorizeController : ControllerBase
    {        
        private readonly VkAuthService _authService;
        private readonly ILogger<VkAuthorizeController> _logger;

        public VkAuthorizeController(VkAuthService authService,
                                     ILogger<VkAuthorizeController> logger)
        {            
            _authService = authService;
            _logger = logger;
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
            return Ok(authUrl);            
        }


        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code,
                                                  [FromQuery] int expires_in,
                                                  [FromQuery] string device_id,
                                                  [FromQuery] string state,
                                                  [FromQuery] string ext_id,
                                                  [FromQuery] string type)
        {            
            /*_logger.LogInformation($"callback code: {code} {Environment.NewLine}" +
                                    $"expires_in: {expires_in}  {Environment.NewLine}" +
                                    $"device_id: {device_id}  {Environment.NewLine}" +
                                    $"state: {state}  {Environment.NewLine}" +
                                    $"ext_id: {ext_id}  {Environment.NewLine}" +
                                    $"type: {type}  {Environment.NewLine}");*/

            try
            {
                TokenResponse tokenResponse = await _authService.HandleCallback(code, state, device_id);                                
                //Перенаправляем пользователя на фронтенд
                return Redirect($"https://virtual-fit.one?" +
                                $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                                $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
