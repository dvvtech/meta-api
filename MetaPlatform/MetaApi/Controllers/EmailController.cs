using MetaApi.Configuration;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Models.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace MetaApi.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _secreteKey;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailSender emailSender,
                               IHttpClientFactory httpClientFactory,
                               IOptions<GoogleRecaptchaConfig> options,
                               ILogger<EmailController> logger)
        {
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
            _secreteKey = options.Value.SecretKey;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            _logger.LogInformation("SE1");
            // Проверяем входные данные
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("All fields (To, Subject, Body) are required.");
            }

            var recaptchaValid = await ValidateRecaptcha(request.RecaptchaToken);
            if (!recaptchaValid)
            {
                _logger.LogInformation("captcha not valid");
                return BadRequest("reCAPTCHA validation failed.");
            }

            _logger.LogInformation("SE2");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {request.Name}");
            sb.AppendLine($"Surname: {request.Surname}");
            sb.AppendLine($"Email: {request.FromEmail}");
            sb.AppendLine($"Body: {request.Body}");

            var res = await _emailSender.SendEmail("dvv153m@gmail.com", request.Subject, sb.ToString());
            _logger.LogInformation("SE3");
            return Ok();
        }

        private async Task<bool> ValidateRecaptcha(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetStringAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={_secreteKey}&response={token}");

            var recaptchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(response);
            return recaptchaResponse?.Success == true && recaptchaResponse.Score >= 0.5;
        }

        private async Task<bool> ValidateRecaptcha2(string token)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", $"{_secreteKey}"),
                new KeyValuePair<string, string>("response", token)
            });

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseBody = await response.Content.ReadAsStringAsync();
            var recaptchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(responseBody);
            return recaptchaResponse?.Success == true && recaptchaResponse.Score >= 0.5;
        }
    }
}
