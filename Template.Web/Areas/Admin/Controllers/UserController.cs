using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Service.DTOs.Admin;
using Template.Service.Implementations;
using Template.Service.Interfaces;
using Template.Utilities.Identity;
using Template.Web.Areas.Admin.ViewModels;

namespace Template.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
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

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);

                return View(user);
            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء استرجاع بيانات المستخدم";
                return View("Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            try
            {
                UserDto user = new();

                ViewBag.Roles = await _userService.GetAllApplicationRolesAsync();
                return View(user);
            }
            catch
            {
                TempData["error"] = "حدث خطأ ما";
                return View("Error");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add( UserDto user, IFormFile userImage)
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

                    user.IsLocked = false;
                    var result = await _userService.AddUserAsync(user);


                    if (result.IsSuccess)
                    {
                        #region Handle image
                        
                       await  _HandleUserImage(user.Id, user, userImage);

                        result = await _userService.UpdateUserAsync(user);
                        if (result.IsSuccess)
                        {
                            TempData["success"] = "تم إضافة المستخدم بنجاح!";

                            return RedirectToAction("Index");
                        }
                    }
                    #endregion

                    TempData["error"] = "حدث خطأ أثناء إضافة المستخدم";
                    return View("Error");
                }

                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(user);

            }
            catch
            {
                ViewBag.Roles = _roleManager.Roles.ToList();

                TempData["error"] = "حدث خطأ أثناء إضافة المستخدم.";
                return View("Error");
            }
        }
 


        [HttpGet]
        public async Task<IActionResult> editRole(int id)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);

                ChangeUserRoleDto changeUserRoleDto = new();
                changeUserRoleDto.Id = user.Id;
                changeUserRoleDto.oldRole = string.Join(", ", user.Role);
                changeUserRoleDto.Roles = (await _userService.GetAllApplicationRolesAsync()).ToList();

                return View(changeUserRoleDto);
            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء استرجاع بيانات المستخدم";
                return View("Error");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editRole(int id, ChangeUserRoleDto changeUserRoleDto)
        {
            try
            {

                var result = await _userService.ChangeUserRoleAsync(id, changeUserRoleDto.oldRole, changeUserRoleDto.newRole);

                if (result.IsSuccess)
                {
                    TempData["success"] = "تم تعديل دور المستخدم بنجاح!";
                    return RedirectToAction("index");
                }
             
                TempData["error"] = "حدث خطأ أثناء تعديل دور المستخدم";
                return View(changeUserRoleDto);

            }
            catch
            {
                TempData["error"] = "حدث خطأ أثناء تعديل دور المستخدم";
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
