using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Global;
using Template.Domain.Identity;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;
using Template.Service.Mapper;
using Template.Utilities.Identity;
using Template.Repository.EntityFrameworkCore.Context;

namespace Template.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private Dictionary<string, List<ApplicationUser>> _roleUserDictionary;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, AppDbContext context, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _roleUserDictionary = new Dictionary<string, List<ApplicationUser>>();
            _mapper = mapper;
            _context = context;
            Task.Run(() => BuildRoleUserDictionary()).Wait();
        }

        private async Task BuildRoleUserDictionary()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                _roleUserDictionary[role.Name] = usersInRole.ToList();
            }
        }
        public async Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize, string? role = null, Expression<Func<ApplicationUser, bool>>? filter = null)
        {
            if (page < 1 || pageSize < 1)
            {
                return new List<UserDto>();
            }

            var usersQuery = _userManager.Users.AsQueryable();

            if (filter != null)
            {
                usersQuery = usersQuery.Where(filter);
            }

            if (!string.IsNullOrEmpty(role))
            {
                var roleId = await _roleManager.Roles
                    .Where(r => r.Name == role)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (roleId == null)
                {
                    return new List<UserDto>();
                }

                usersQuery = from user in usersQuery
                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                             where userRole.RoleId == roleId
                             select user;
            }

            var paginatedUsers = await usersQuery
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = new List<UserDto>();
            foreach (var user in paginatedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = await UserDTOsMapper.ToUserDto(user, Task.FromResult(roles));
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return null;
                }

                var roleNames = await _userManager.GetRolesAsync(user);

                return await UserDTOsMapper.ToUserDto(user, Task.FromResult(roleNames));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return null;
            }
        }

        public async Task<UserDto?> GetAdminUserAsync(int userId, string username)
        {
            try
            {
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    return null;
                }

                var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name);

                var adminUser = adminUsers.SingleOrDefault(x => x.Id == userId);

                if (adminUser == null)
                {
                    return null;
                }

                return await UserDTOsMapper.ToUserDto(adminUser, _userManager.GetRolesAsync(adminUser));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return null;
            }
        }
        public async Task<Result> AddUserAsync(UserDto userDto)
        {
            if (userDto == null)
            {
                return Result.Failure("بيانات المستخدم غير صحيحة");
            }

            if (string.IsNullOrWhiteSpace(userDto.FirstName))
            {
                return Result.Failure("الاسم الأول مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.LastName))
            {
                return Result.Failure("اسم العائلة مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                return Result.Failure("البريد الإلكتروني مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                return Result.Failure("اسم المستخدم مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Phone))
            {
                return Result.Failure("رقم الهاتف مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                return Result.Failure("كلمة المرور مطلوبة");
            }

            if (userDto.Role == null || !userDto.Role.Any())
            {
                return Result.Failure("الدور مطلوب");
            }

            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
            {
                return transactionResult;
            }

            try
            {
                var user = UserDTOsMapper.ToApplicationUser(userDto);

                var result = await _userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, userDto.Role.FirstOrDefault());
                    if (!roleResult.Succeeded)
                    {
                        await _unitOfWork.RollbackAsync();
                        return Result.Failure(roleResult.Errors.Select(e => e.Description).FirstOrDefault());
                    }

                    var commitResult = await _unitOfWork.CommitAsync();
                    if (!commitResult.IsSuccess)
                    {
                        await _unitOfWork.RollbackAsync();
                        return commitResult;
                    }

                    return Result.Success();
                }

                await _unitOfWork.RollbackAsync();
                return Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure($"فشل في إضافة المستخدم: {ex.Message}");
            }
        }

        public async Task<Result> UpdateUserAsync(UserDto userDto)
        {
            if (userDto == null)
            {
                return Result.Failure("بيانات المستخدم غير صحيحة");
            }

            if (userDto.Id <= 0)
            {
                return Result.Failure("معرف المستخدم غير صحيح");
            }

            if (string.IsNullOrWhiteSpace(userDto.FirstName))
            {
                return Result.Failure("الاسم الأول مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.LastName))
            {
                return Result.Failure("اسم العائلة مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                return Result.Failure("البريد الإلكتروني مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                return Result.Failure("اسم المستخدم مطلوب");
            }

            if (string.IsNullOrWhiteSpace(userDto.Phone))
            {
                return Result.Failure("رقم الهاتف مطلوب");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

                if (user == null)
                {
                    return Result.Failure("المستخدم غير موجود");
                }

                // Perform business logic/validation here

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.UserName = userDto.UserName;
                user.PhoneNumber = userDto.Phone;
                user.ImagePath = userDto.ImagePath;

                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"فشل في تحديث المستخدم: {ex.Message}");
            }
        }


        public async Task<Result> LockUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var isAdmin = await _userManager.GetRolesAsync(user);

                if(isAdmin.Contains(AppUserRoles.RoleAdmin))
                {
                    return Result.Failure("لا تستطيع ان تنفذ هذه العملية على الادمن");
                }

                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Lockout indefinitely
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to lock user: {ex.Message}");
            }
        }

        public async Task<Result> UnlockUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                user.LockoutEnd = null; // Remove lockout
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to unlock user: {ex.Message}");
            }
        }

        public async Task<Result> EditUserRoleAsync(int userId, string newRole)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    return Result.Failure(removeResult.Errors.Select(e => e.Description).FirstOrDefault());
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);

                return addResult.Succeeded ? Result.Success() : Result.Failure(addResult.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to edit user role: {ex.Message}");
            }
        }

        public async Task<Result> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var result = await _userManager.DeleteAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"Failed to delete user: {ex.Message}");
            }
        }
    }
}
