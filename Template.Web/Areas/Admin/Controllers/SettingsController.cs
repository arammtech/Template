using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;
using Template.Utilities.Identity;

namespace Template.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    public class SettingsController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public SettingsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,  IUserService userService, IWebHostEnvironment webHostEnvironment)
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
                UserDto admin = await _userService.GetUserByIdAsync(1);

                return View(admin);
            }
            catch
            {
                return RedirectToAction("Index");
            }


        }
        [HttpGet]
        public async Task<IActionResult> editProfile(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                //ViewBag.Roles = _roleManager.Roles.ToList();
                //ViewBag.Roles = await _userService.GetAllApplicationRolesAsync();

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

                    //ViewBag.Roles = await _userService.GetAllApplicationRolesAsync();
                    TempData["error"] = "حدث خطأ أثناء تعديل بيانات المستخدم";
                    return View(user);
                }

                //ViewBag.Roles = await _userService.GetAllApplicationRolesAsync();
                return View(user);
            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء تعديل بيانات المستخدم";
                return View("Error");

            }
        }

        private async Task _HandleUserImage(int userId, UserDto user, IFormFile? mainImage)
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
