using MetaApi.Models.Email;

namespace MetaApi.Services.Interfaces
{
    public interface IEmailBodyGenerator
    {
        string GenerateEmailBody(SendEmailRequest request);
    }
}
