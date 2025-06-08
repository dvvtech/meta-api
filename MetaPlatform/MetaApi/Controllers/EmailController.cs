using MetaApi.Configuration;
using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Models.Email;
using MetaApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MetaApi.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<GoogleRecaptchaConfig> _recaptchaOptions;
        private readonly IEmailBodyGenerator _emailBodyGenerator;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailSender emailSender,
                               IHttpClientFactory httpClientFactory,
                               IOptions<GoogleRecaptchaConfig> recaptchaOptions,
                               IEmailBodyGenerator emailBodyGenerator,
                               ILogger<EmailController> logger)
        {
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
            _recaptchaOptions = recaptchaOptions;
            _emailBodyGenerator = emailBodyGenerator;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {            
            // Проверяем входные данные
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("All fields (To, Subject, Body) are required.");
            }

            string secretKey = request.Type switch
            {
                1 => _recaptchaOptions.Value.SecretKeyForOxfordAp,
                2 => _recaptchaOptions.Value.SecretKeyForYashelCenter,
                _ => throw new ArgumentException("Invalid Type")
            };

            var recaptchaValid = await ValidateRecaptcha(request.RecaptchaToken, secretKey);
            if (!recaptchaValid)
            {
                _logger.LogInformation("captcha not valid");
                return BadRequest("reCAPTCHA validation failed.");
            }

            var emailBody = _emailBodyGenerator.GenerateEmailBody(request);

            var res = await _emailSender.SendEmail("dvv153m@gmail.com", request.Subject, emailBody);            
            return Ok();
        }

        private async Task<bool> ValidateRecaptcha(string token, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetStringAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}");

            var recaptchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(response);
            return recaptchaResponse?.Success == true && recaptchaResponse.Score >= 0.5;
        }

        private async Task<bool> ValidateRecaptcha2(string token, string secretKey)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", $"{secretKey}"),
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
