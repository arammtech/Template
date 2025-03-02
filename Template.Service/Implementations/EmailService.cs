using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Utilities.Net;
using System.Net;
using Template.Domain.Entities;
using Template.Domain.Identity;
using Template.Service.Global;
using Template.Service.Interfaces;

namespace Template.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings, UserManager<ApplicationUser> userManger)
        {
            _emailSettings = emailSettings.Value;
            _userManager = userManger;
        }


        public async Task<string> GenerateToken(ApplicationUser user)
        {
            if (user == null) { return ""; }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenRecord = new TokenRecord
            {
                Token = token,
                UserId = user.Id,
                //IPAddress = ipAddress,
                ExpirationDate = DateTime.UtcNow.AddSeconds(59) // Token expires in 1 hour
            };

            return tokenRecord;
            
        }
        public async Task<string> GenerateLinkToVerifyTokenAsync(string token, int userId)
        {
            var encodedToken = WebUtility.UrlEncode(token);
            // Fix the URL by removing the extra 'http://' part
            return $"http://127.0.0.1:5500/index.html?userId={userId}&token={encodedToken}";
            //return $"<a href='{verificationLink}'>here</a>";
        }
        public async Task<Result> SendEmailAsync(string recipient, string recipientName, string subject, string body, string senderName = "ARAMM")
        {
            try
            {
                //body = "<html><body><p>Click the link below:</p><a href='https://www.example.com'>Visit Example</a></body></html>";
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(recipientName, recipient));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.AppPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                return new Result(ex.Message, false);
            }

            return new Result("Send Succefully", true);
        }
        public async Task<Result> VerifyEmailAsync(int userId, string token)
        {

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new Result("User not found.", false);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            
            if (result.Succeeded)
                return new Result("Email Verfied", true);

            return new Result("Email Not Verified", false);

        }

        
    }

}
