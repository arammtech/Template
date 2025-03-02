using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Template.Utilities.Identity
{
    public class EmailSender : IEmailSender
    {
        // Should be encrypted Later
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "teamaramm@gmail.com";
        private readonly string _smtpPass = "iweq ccca vufw gbnb";
        private readonly string _fromEmail = "teamaramm@gmail.com";

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
