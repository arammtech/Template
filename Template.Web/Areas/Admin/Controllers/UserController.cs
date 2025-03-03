using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Utilities.Identity;

namespace Template.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager,AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try {
                //var lstUsers = (await _userManager.GetUsersInRoleAsync(AppUserRoles.RoleCustomer)).Select(user => new
                //{
                //    user.Id,
                //    user.UserName,
                //    user.Email
                //}).ToList();

                var lstUsers = _context.ApplicationUsers.Select(user => new
                {
                    user.Id,
                    Name = user.FirstName + user.LastName,
                    user.Email,
                    Phone = user.PhoneNumber,
                    // Retrieve the user's role by joining the UserRoles and Roles tables.
                    Role = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == user.Id
                            select r.Name).FirstOrDefault(),
                    // Determine if the user is locked by checking LockoutEnd.
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow
                }).ToList();

                return Ok(new { success = true, data = lstUsers });
            }
            catch (Exception ex)
            {
                // Log the exception details (optional)
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving users." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> LockUnLock([FromBody] int userId)
        {
            try
            {
                var userFromDb = await _userManager.GetUserAsync(User);

                if (userFromDb == null)
                {
                    return Json(new { success = false, message = $"There is no user with the Id = {userId}" });
                }

                // if this user is the main user (admin) return false because we can't lock the main user
                if (!userFromDb.LockoutEnabled)
                {
                    return Json(new { success = false, message = "Ooobs! You cannot lock this user, they are the main user in this system." });
                }

                string result = "";

                // if the user is already locked we need to unlock them, otherwise lock the user
                if (userFromDb.LockoutEnd > DateTime.Now)
                {
                    // unlock the user
                    userFromDb.LockoutEnd = DateTime.Now;
                    result = "unlocked";
                }
                else
                {
                    // lock the user for a 1000 years
                    userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                    result = "locked";
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"User has been {result} successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception details (optional)
                return StatusCode(500, new { success = false, message = "An error occurred while lock/unLock user." });
            }
        }
        #endregion

    }
}
