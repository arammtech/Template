
using Template.Domain.Global;

namespace Template.Service.EmailService
{
    public interface IEmailSenderStrategy
    {
        Task<Result> SendEmailAsync(string toMail,string toName, string subject, string body);
    }
}
