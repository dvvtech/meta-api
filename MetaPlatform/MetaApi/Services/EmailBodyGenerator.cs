using MetaApi.Models.Email;
using MetaApi.Services.Interfaces;
using System.Text;

namespace MetaApi.Services
{
    public class EmailBodyGenerator : IEmailBodyGenerator
    {
        public string GenerateEmailBody(SendEmailRequest request)
        {
            return request.Type switch
            {
                1 => GenerateBodyForOxfordAp(request),
                2 => GenerateTypeFromYashelCenter(request),
                _ => GenerateDefaultBody(request)
            };
        }

        private string GenerateBodyForOxfordAp(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<h3>Contact Form https://oxford-ap.com/</h3>");
            sb.AppendLine($"<p><strong>Name:</strong> {request.Name}</p>");
            sb.AppendLine($"<p><strong>Surname:</strong> {request.Surname}</p>");
            sb.AppendLine($"<p><strong>Email:</strong> {request.FromEmail}</p>");
            sb.AppendLine($"<p><strong>Message:</strong></p><p>{request.Body}</p>");
            return sb.ToString();
        }

        private string GenerateTypeFromYashelCenter(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<h3>Contact Form https://yashel.center/</h3>");
            sb.AppendLine($"<p><strong>Name:</strong> {request.Name}</p>");
            sb.AppendLine($"<p><strong>Surname:</strong> {request.Surname}</p>");
            sb.AppendLine($"<p><strong>Email:</strong> {request.FromEmail}</p>");
            sb.AppendLine($"<p><strong>Message:</strong></p><p>{request.Body}</p>");
            return sb.ToString();
        }

        private string GenerateDefaultBody(SendEmailRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<h3>New Message</h3>");
            sb.AppendLine($"<p><strong>From:</strong> {request.Name} {request.Surname}</p>");
            sb.AppendLine($"<p><strong>Email:</strong> {request.FromEmail}</p>");
            sb.AppendLine($"<p><strong>Content:</strong></p><p>{request.Body}</p>");
            return sb.ToString();
        }
    }
}
