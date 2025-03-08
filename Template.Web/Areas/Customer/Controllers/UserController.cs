using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Template.Domain.Identity;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;

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

                var user = await _userService.GetUserByIdAsync(userId);

                return View(user);
            }
            catch
            {
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
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> editProfile(int Id, UserDto user, IFormFile userImage)
        {
            try
            {
               if(ModelState.IsValid)
                {
                    var result = await _userService.UpdateUserAsync(user);

                    if (result.IsSuccess)
                    {
                        return View(user);

                    }
                }


                return View(user);
            }
            catch
            {
                return View("Error");

            }
        }
        public async Task<IActionResult> Delete()
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
                int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

                var user = await _userService.GetUserByIdAsync(userId);

                return View(user);
            }
            catch
            {
                return View("Error");

            }
        }


    }
}
