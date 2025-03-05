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
                var lstUsers = _context.ApplicationUsers.Select(user => new
                {
                    user.Id,
                    Name = user.FirstName + user.LastName,
                    user.Email,
                    Phone = user.PhoneNumber,
                    Role = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == user.Id
                            select r.Name).FirstOrDefault(),
                    // Compare using UtcDateTime to ensure proper UTC comparison
                    IsLocked = user.LockoutEnd != null &&  user.LockoutEnd > DateTime.Now
                }).ToList();

                // (Optional) Apply filtering if needed

                return Ok(new { success = true, data = lstUsers });
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving users." });
            }
        }

    
        [HttpPost("LockUnLock")]
        public async Task<IActionResult> LockUnLock([FromQuery] int userId)
        {
            try
            {
                // Find the user by Id (adjust depending on your user ID type)
                var userFromDb = await _userManager.FindByIdAsync(userId.ToString());

                if (userFromDb == null)
                {
                    return NotFound(new { success = false, message = $"There is no user with the Id = {userId}" });
                }

                // Prevent locking the main admin user
                if (!userFromDb.LockoutEnabled)
                {
                    return BadRequest(new { success = false, message = "Oops! You cannot lock this user, they are the main user in this system." });
                }

                string result;

                // If the user is already locked, unlock them; otherwise, lock them
                if (userFromDb.LockoutEnd > DateTime.Now)
                {
                    userFromDb.LockoutEnd = DateTime.Now; // Unlock user
                    result = "unlocked";
                }
                else
                {
                    userFromDb.LockoutEnd = DateTime.Now.AddYears(1000); // Lock user for 1000 years
                    result = "locked";
                }

                await _userManager.UpdateAsync(userFromDb); // Save changes via UserManager

                return Ok(new { success = true, message = $"User has been {result} successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while locking/unlocking the user." });
            }
        }




    }
}
