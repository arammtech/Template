﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MimeKit;
using Template.Domain.Global;
using Template.Domain.Identity;

namespace Template.Service.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(string recipient, string recipientName, string subject, string body, string senderName = "ARAMM");
        Task<string> GenerateLinkToVerifyTokenAsync(string token, int userId);
        Task<string> GenerateToken(ApplicationUser user);
        Task<Result> VerifyEmailAsync(int userId, string token);
    }
}
