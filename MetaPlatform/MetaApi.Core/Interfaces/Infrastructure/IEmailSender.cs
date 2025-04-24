using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Core.Interfaces.Infrastructure
{
    public interface IEmailSender
    {
        Task<Result> SendEmail(string to, string subject, string body);
    }
}
