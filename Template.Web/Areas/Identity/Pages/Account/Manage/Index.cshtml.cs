// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Template.Domain.Identity;
using Template.Service.Interfaces;

namespace Template.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserService _userService;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
        }

     
        
        [TempData]
        public string StatusMessage { get; set; }

       
        [BindProperty]
        public InputModel Input { get; set; }

       
        public class InputModel
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "الاسم الأول مطلوب")]
            [DisplayName("الاسم الأول ")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "الاسم الأخير مطلوب")]
            [DisplayName("الاسم الأخير")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
            [DisplayName("البريد الإلكتروني")]
            [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
            public string Email { get; set; }
            public string? UserName { get; set; }
            [StringLength(11, MinimumLength = 11, ErrorMessage = "رقم الهاتف يجب أن يتألف من 11 خانة حصرا")]
            [DisplayName("رقم الهاتف )")]
            [Required(ErrorMessage = "مطلوب رقم الهاتف )")]
            public string? Phone { get; set; }
            // make here validation
            [Required(ErrorMessage = "كلمة المرور مطلوبة")]
            [DisplayName("كلمة المرور")]
            [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "يجب أن تحتوي كلمة المرور على 8 أحرف على الأقل، حرف كبير، رقم، وحرف خاص.")]
            public string? Password { get; set; }
            public string? ImagePath { get; set; }
            [Required(ErrorMessage = "الدور مطلوب")]
            [DisplayName("الدور")]
            public List<string> Role { get; set; }
            public bool? IsLocked { get; set; }

        }

        //private async Task LoadAsync(ApplicationUser user)
        //{
        //    var userName = await _userManager.GetUserNameAsync(user);
        //    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        //    Input.UserName = userName;

        //    Input = new InputModel
        //    {
        //        Phone = phoneNumber
        //    };
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
            int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.Phone,
                ImagePath = user.ImagePath,
                Role = user.Role,
                IsLocked = user.IsLocked
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {



                //await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.Phone != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Phone);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
