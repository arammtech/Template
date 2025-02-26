// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Template.Utilities.Global;
using Template.Domain.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Template.Web.Areas.Identity.Pages.Account
{
    

    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordConfirmation(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        //[BindProperty]
        //public InputModel Input { get; set; } = new();

        //public class InputModel
        //{

        //    public string Email { get; set; }
        //    public int resendCodeTimeMins { get; set; }

        //}

        public void OnGet()
        {
            //store the time where ??
            //in session??
            //Input.resendCodeTimeMins = GlobalFunctions.GetResendCodeTime(1);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                return RedirectToPage("/Account/ResetPassword");

                //var user = await _userManager.FindByEmailAsync(Input.Email);
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return RedirectToPage("/Account/ResetPassword");
                //}

                //return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
