using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;

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
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<IActionResult> editRole(int id)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);

                ChangeUserRoleDto changeUserRoleDto = new();
                changeUserRoleDto.userId = user.Id;
                changeUserRoleDto.oldRole = string.Join(", ", user.Role);
                changeUserRoleDto.newRole = string.Join(", ", user.Role);
                changeUserRoleDto.Roles = _roleManager.Roles.ToList();

                return View(changeUserRoleDto);
            }
            catch
            {
                return RedirectToAction("Index");
            }
          
        }

        [HttpPost]
        public async Task<IActionResult> editRole(int id, ChangeUserRoleDto changeUserRoleDto)
        {
            try
            {
                UserDto user = await _userService.GetUserByIdAsync(id);

                

                return RedirectToAction("index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
