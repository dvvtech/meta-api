using MetaApi.Core.Interfaces.Infrastructure;
using MetaApi.Models.Email;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MetaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            // Проверяем входные данные
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("All fields (To, Subject, Body) are required.");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {request.Name}");
            sb.AppendLine($"Surname: {request.Surname}");
            sb.AppendLine($"Email: {request.FromEmail}");
            sb.AppendLine($"Body: {request.Body}");

            var res = await _emailSender.SendEmail("dvv153m@gmail.com", request.Subject, sb.ToString());
            return Ok();
        }
    }
}
