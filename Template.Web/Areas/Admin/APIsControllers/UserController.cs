using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Service.DTOs.Admin;
using Template.Service.Implementations;
using Template.Service.Interfaces;
using Template.Service.Mapper;
using Template.Utilities.Identity;
using static System.Reflection.Metadata.BlobBuilder;
using AutoMapper;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Global;

namespace Template.Web.Areas.Admin.APIsControllers
{
    [Authorize(Roles = AppUserRoles.RoleAdmin)]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;

      
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, AppDbContext context, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _context = context;

        }



        //C:\Users\meryk\AppData\Local\SourceServer\13a535b10cec5cb2768d6ab5c80731f49d6b54dc56d6977252e531ad338caf07\Template.Web\Areas\Admin\APIsControllers\UserController.cs
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int rowsPerPage, [FromQuery] string? filterProperty, [FromQuery] string? filter)
        {
            List<UserDto> lstUsers = [];
            (IEnumerable<UserDto> Users, int TotalRecords) result;
            int usersCount = 0;
            bool? isLocked = null;
            string? role = null;

            try
            {
                // apply filter
                if (!(string.IsNullOrEmpty(filterProperty) && string.IsNullOrEmpty(filter)))
                {
                    if (filterProperty == "role")
                    {
                        role = filter;
                        result = (await _userService.GetUsersAsync(page, rowsPerPage, role));
                        lstUsers = result.Users.ToList();
                        usersCount = result.TotalRecords;

                    }
                    else if (filterProperty == "isLocked")
                    {
                        isLocked = filter.ToLower() == "true";
                        result = (await _userService.GetUsersAsync(page, rowsPerPage, null, null, isLocked));
                        lstUsers = result.Users.ToList();
                        usersCount = result.TotalRecords;

                    }

                    return Ok(new { success = true, data = lstUsers, totalUsers = usersCount });
                }

                result = await _userService.GetUsersAsync(page, rowsPerPage);
                lstUsers = result.Users.ToList();
                usersCount = result.TotalRecords;


                return Ok(new { success = true, data = lstUsers, totalUsers = usersCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء استرجاع المستخدمين." });
            }
        }

        [HttpPost("LockUnLock")]
        public async Task<IActionResult> LockUnLock([FromQuery] int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return NotFound(new { success = false, message = $"لا يوجد مستخدم بالمعرّف = {userId}" });
                }


                if (!user.LockoutEnabled)
                {
                    return BadRequest(new { success = false, message = "عذراً! لا يمكنك قفل هذا المستخدم، فهو المستخدم الرئيسي في هذا النظام." });
                }

                string result = "";

                if (user.LockoutEnd > DateTime.Now)
                {
                    var Result = await _userService.UnlockUserAsync(userId);

                    if (Result.IsSuccess) result = "فك القفل";
                    else return StatusCode(500, new { success = false, message = "حدث خطأ أثناء فك قفل المستخدم." });
                }
                else
                {
                    var Result = await _userService.LockUserAsync(userId);

                    if (Result.IsSuccess) result = "قُفل";
                    else return StatusCode(500, new { success = false, message = "حدث خطأ أثناء قفل المستخدم." });
                }

                return Ok(new { success = true, message = $"تم {result} المستخدم بنجاح!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء قفل/فك قفل المستخدم." });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = $"({id}) هو معرف غير صالح" });

            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result.IsSuccess)
                {
                    return Ok(new { success = true, message = "تم حذف المستخدم بنجاح!" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "حدث خطأ أثناء حذف المستخدم." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء حذف المستخدم." });
            }
        }






    }
}

//var lstUsers = _context.ApplicationUsers.Select(user => new
//{
//    user.Id,
//    Name = user.FirstName + user.LastName,
//    user.Email,
//    Phone = user.PhoneNumber,
//    Role = (from ur in _context.UserRoles
//            join r in _context.Roles on ur.RoleId equals r.Id
//            where ur.UserId == user.Id
//            select r.Name).FirstOrDefault(),
//    // Compare using UtcDateTime to ensure proper UTC comparison
//    IsLocked = user.LockoutEnd != null &&  user.LockoutEnd > DateTime.Now
//}).ToList();
