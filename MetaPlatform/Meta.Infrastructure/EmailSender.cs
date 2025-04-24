using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MetaApi.Core.Interfaces.Infrastructure;

namespace Meta.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _emailPassword;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(string smtpServer,
                           int smtpPort,
                           string fromEmail,
                           string emailPassword,
                           ILogger<EmailSender> logger)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _fromEmail = fromEmail;
            _emailPassword = emailPassword;
            _logger = logger;
        }

        public async Task<Result> SendEmail(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("DVV", _fromEmail));
                mailMessage.To.Add(new MailboxAddress("dvv", to));
                mailMessage.Subject = subject;
                mailMessage.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(_smtpServer, _smtpPort, useSsl: true);
                    await smtpClient.AuthenticateAsync(_fromEmail, _emailPassword);
                    await smtpClient.SendAsync(mailMessage);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email. Error: {ex.Message}");
                return Result.Failure(UserErrors.EmailSendError());
            }

            return Result.Success();
        }
    }
}
