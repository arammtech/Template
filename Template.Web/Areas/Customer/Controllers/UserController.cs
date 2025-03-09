using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using Template.Domain.Global;
using Template.Domain.Identity;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;
using Template.Web.Areas.Admin.ViewModels;

namespace Template.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;
         private readonly IWebHostEnvironment _webHostEnvironment;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUserService userService, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;

        }

    

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
                int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

                var userDto = await _userService.GetUserByIdAsync(userId);
                var AppUser = await _userManager.GetUserAsync(User);

                // For security,if the user is not confirmed force him to confirm his account
                if (!(await _userManager.IsEmailConfirmedAsync(AppUser)))
                {
                    ViewBag.EmailConfirmNeeded = "أضغط لتأكيد حسابك.";
                }

                // Generate an email confirm token
                var emailCode = await _userManager.GenerateEmailConfirmationTokenAsync(AppUser);
                ViewBag.EmailCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailCode));

                // Generate a password reset token
                var passwordCode = await _userManager.GeneratePasswordResetTokenAsync(AppUser);
                ViewBag.PasswordCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordCode));


                return View(userDto);
            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء استرجاع بيانات المستخدم";
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> editProfile(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                return View(user);
            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء استرجاع بيانات المستخدم";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editProfile(int Id, UserDto user, IFormFile userImage)
        {
            try
            {
                // remove require validation on the image
                if (ModelState.ErrorCount == 1 && ModelState.ContainsKey(nameof(userImage)))
                {
                    ModelState.Remove(nameof(userImage));
                }

                if (ModelState.IsValid)
                {
                    var oldUser = (await _userService.GetUserByIdAsync(Id));
                    string imagePath = oldUser.ImagePath;

                   
                    if (imagePath != null)
                    {
                      
                        //// Delete old image
                        string wwwRootPath = _webHostEnvironment.WebRootPath;

                        string fullPath = Path.Combine(wwwRootPath, imagePath.Trim('\\'));

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    await _HandleUserImage(user.Id, user, userImage);

                    var result = await _userService.UpdateUserAsync(user);


                    if (result.IsSuccess)
                    {
                        TempData["success"] = "تم تعديل بيانات المستخدم بنجاح!";
                        return RedirectToAction("Profile");
                    }

                    TempData["error"] = "حدث خطأ أثناء تعديل بيانات المستخدم";
                    return View(user);
                }


                return View(user);
            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء تعديل بيانات المستخدم";
                return View("Error");

            }
        }


        public  async Task _HandleUserImage(int userId, UserDto user, IFormFile? mainImage)
        {
            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string imagePath = @"uploads\images\users\user-" + userId;
                string finalPath = Path.Combine(wwwRootPath, imagePath);

                if (!Directory.Exists(finalPath))
                    Directory.CreateDirectory(finalPath);

                if (mainImage != null)
                {
                    await _CopayImage(mainImage, true);
                }


                async Task _CopayImage(IFormFile file, bool isMainImage = false)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }


                    user.ImagePath = @"\" + imagePath + @"\" + fileName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
