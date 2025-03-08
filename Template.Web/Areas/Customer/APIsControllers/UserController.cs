using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;

namespace Template.Web.Areas.Customer.APIsControllers
{
    [Route("api/customer/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
      

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
          
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result.IsSuccess)
                {
                    await _signInManager.SignOutAsync();
                    return Ok(new { success = true, message = "Account deleted successfully!" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "An error occurred while deleting the user." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the user." });
            }
        }



    }
}
