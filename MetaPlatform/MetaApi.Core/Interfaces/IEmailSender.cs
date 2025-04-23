using MetaApi.Core.OperationResults.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.Core.Interfaces
{
    public interface IEmailSender
    {
        Task<Result> SendEmail(string to, string subject, string body);
    }
}
