using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Org.BouncyCastle.Security;
using System.Net.Mail;
using System.Threading.Tasks;
using Template.Service.Global;

namespace Template.Service.Email
{
    public class EmailService
    {
        private readonly string _smtpServer; // Change for other providers
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail;
        private readonly string _appPassword;

        public EmailService(string senderEmail, string appPssword, string smtpServer)
        {
            _appPassword = appPssword;
            _senderEmail = senderEmail;
            _smtpServer = smtpServer;
        }

        public async Task<Result> SendEmailAsync(string recipient, string subject, string body, string recipientName, string senderName = "ARAMM")
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, _senderEmail));
                message.To.Add(new MailboxAddress(recipientName, recipient));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_senderEmail, _appPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                return new Result(ex.Message, false);
            }

            return new Result("Send Succefully", true);
        }

        public async Task<Result> VerifyEmailAsync(string recipient, string subject, string body, string recipientName, string senderName = "ARAMM")
        {
           
            try
            {
                var result =  await SendEmailAsync(recipient, subject, body, recipientName, senderName);
            }
            catch (System.Exception ex)
            {
                return new Result(ex.Message, false);
            }
            
            return new Result("Send Succefully", true);

        }

        
    }

}
