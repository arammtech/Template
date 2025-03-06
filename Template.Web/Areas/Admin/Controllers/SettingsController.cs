using Microsoft.AspNetCore.Authorization;
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


        public SettingsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,  IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;

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
    }
}
