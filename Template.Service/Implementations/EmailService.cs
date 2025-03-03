using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Utilities.Net;
using System.Net;
using System.Net.Http;
using System.Text;
using Template.Domain.Entities;
using Template.Domain.Global;
using Template.Domain.Identity;
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
            if (user == null) { return null; }
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<string> GenerateLinkToVerifyTokenAsync(string token, int userId)
        {

            var encodedToken = WebUtility.UrlEncode(token);
            return $"http://127.0.0.1:5500/index.html?userId={userId}&token={_ShortenToken(encodedToken)}";
        }
        public async Task<Result> SendEmailAsync(string recipient, string recipientName, string subject, string body, string senderName = "ARAMM")
        {
            try
            {
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
                return Result.Failure(ex.Message);
            }

            return  Result.Success();
        }
        public async Task<Result> VerifyEmailAsync(int userId, string token)
        {
            token = _DecodeShortToken(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return  Result.Failure("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            
            if (result.Succeeded)
                return Result.Success();

            return  Result.Failure("Email Not Verified");

        }

        private string _DecodeShortToken(string shortToken)
        {
            string paddedToken = shortToken
                .Replace('-', '+')
                .Replace('_', '/');

            while (paddedToken.Length % 4 != 0)
                paddedToken += "="; // Restore padding

            var tokenBytes = Convert.FromBase64String(paddedToken);
            return Encoding.UTF8.GetString(tokenBytes);
        }

        private string _ShortenToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(tokenBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('='); // Removes padding
        }




    }

}
