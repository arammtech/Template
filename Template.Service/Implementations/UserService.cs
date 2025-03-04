using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Identity;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;
using static Template.Domain.Global.Result;

namespace Template.Service.Implementations
{
    public partial class EmployeeService
    {
        public class UserService : IUserService
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public UserService(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<PaginatedResult<UserDto>> GetUsersAsync(int page, int pageSize)
            {
                int totalUsers = await _userManager.Users.CountAsync();
                if (page < 1 || pageSize < 1) { return new PaginatedResult<UserDto>(new List<UserDto>(), 0, page, pageSize); }

                var usersPaged = await _userManager.Users
                  .OrderBy(u => u.Id)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .Select(u => new UserDto
                  {
                      Id = u.Id,
                      Name = $"{u.FirstName} {u.LastName}"
                  }).ToListAsync();


                return new PaginatedResult<UserDto>(usersPaged, totalUsers, page, pageSize);
            }
        }

    }

}