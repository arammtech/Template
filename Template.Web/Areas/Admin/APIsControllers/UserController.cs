using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Utilities.Identity;
using static System.Reflection.Metadata.BlobBuilder;

namespace Template.Web.Areas.Admin.APIsControllers
{
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterProperty, [FromQuery] string? filter)
        {
            try
            {
                var lstUsers = (await _userManager.GetUsersInRoleAsync(AppUserRoles.RoleCustomer)).Select(user => new
                {
                    user.Id,
                    Name = user.FirstName + user.LastName,
                    user.Email,
                    Phone = user.PhoneNumber,
                    Role = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == user.Id
                            select r.Name).FirstOrDefault(),
                    // Determine if the user is locked by checking LockoutEnd.
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow
                }).ToList();


                //var lstUsers = _context.ApplicationUsers.Select(user => new
                //{
                //    user.Id,
                //    Name = user.FirstName + user.LastName,
                //    user.Email,
                //    Phone = user.PhoneNumber,
                //    // Retrieve the user's role by joining the UserRoles and Roles tables.
                //    Role = (from ur in _context.UserRoles
                //            join r in _context.Roles on ur.RoleId equals r.Id
                //            where ur.UserId == user.Id
                //            select r.Name).FirstOrDefault(),
                //    // Determine if the user is locked by checking LockoutEnd.
                //    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow
                //}).ToList();

                if (!string.IsNullOrEmpty(filterProperty))
                {
                    //filter
                }

                return Ok(new { success = true, data = lstUsers });
            }
            catch (Exception ex)
            {
                // Log the exception details (optional)
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving users." });
            }
        }





    }
}
