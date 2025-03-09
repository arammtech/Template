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
using Template.Web.Areas.Admin.ViewModels;
using static Template.Service.MostUses.Validations.UserValidation;
using static Template.Service.MostUses.Generates.UserGenerate;
namespace Template.Service.Implementations
{

    public class UserService : IUserService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, AppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        public async Task<(IEnumerable<UserDto>? Users, int? TotalRecords)> GetUsersAsync(
            int page,
            int pageSize = 10,
            string? role = null,
            Expression<Func<ApplicationUser, bool>>? filter = null,
            bool? isLocked = null)
        {
            if (page < 1 || pageSize < 1)
            {
                return (null, null);
            }
           
            try
            {
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
                        return (null, null);
                    }

                    usersQuery = from user in usersQuery
                                 join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                 where userRole.RoleId == roleId
                                 select user;

                }

                if (isLocked.HasValue)
                {
                    usersQuery = isLocked.Value
                        ? usersQuery.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow)
                        : usersQuery.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= DateTimeOffset.UtcNow);
                }

                var totalRecords = await usersQuery.CountAsync(); 

                var paginatedUsers = await usersQuery
                    .OrderBy(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(user => new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        IsLocked = user.LockoutEnd.HasValue ? user.LockoutEnd.Value > DateTimeOffset.UtcNow : false,
                        Role = (from userRole in _context.UserRoles
                                join role in _context.Roles on userRole.RoleId equals role.Id
                                where userRole.UserId == user.Id
                                select role.Name).ToList()
                    })
                    .ToListAsync();

                return (paginatedUsers, totalRecords);

            } catch (Exception ex)
            {
                return (null, null);
            }
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

        public async Task<UserDto?> GetAdminUserAsync(int userId)
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
            // Validate input
            if (userDto == null)
            {
                return Result.Failure("بيانات المستخدم غير صحيحة");
            }


            var result = ValidateRequiredFields(userDto);

            if (result.HasErrors)
            {
                return result; // Return errors if validation fails
            }

            // Check email uniqueness
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return Result.Failure("البريد الإلكتروني مسجل بالفعل");
            }

            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
            {
                return Result.Failure("فشل في بدء المعاملة");
            }

            try
            {
                // Map DTO to application user
                var user = UserDTOsMapper.ToApplicationUser(userDto);

                // Create user
                var creationResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (!creationResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure("فشل في إضافة المستخدم");
                }

                // Assign role to user
                var roleResult = await _userManager.AddToRoleAsync(user, userDto.Role.FirstOrDefault());
                if (!roleResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure("فشل في إضافة الدور للمستخدم");
                }

                // Commit transaction
                var commitResult = await _unitOfWork.CommitAsync();
                if (!commitResult.IsSuccess)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure("فشل في إتمام المعاملة");
                }

                userDto.Id = user.Id; 
                return Result.Success(); 
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure($"خطأ غير متوقع: {ex.Message}");
            }
        }

        public async Task<Result> UpdateUserAsync(UserDto userDto)
        {
            // Validate input
            if (userDto == null)
                return Result.Failure("بيانات المستخدم غير صحيحة");

            if (userDto.Id <= 0)
                return Result.Failure("معرف المستخدم غير صحيح");

            var result = ValidateRequiredFields(userDto);

            if (result.HasErrors)
            {
                return result; // Return errors if validation fails
            }

            try
            {
                // Check if user exists
                var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
                if (user == null)
                    return Result.Failure("المستخدم غير موجود");

                // Check if Email is unique (excluding current user)
                var existingUserByEmail = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUserByEmail != null && existingUserByEmail.Id != userDto.Id)
                    return Result.Failure("البريد الإلكتروني مسجل بالفعل");

                if(!string.IsNullOrEmpty(user.UserName))
                {
                    // Check if Email is unique (excluding current user)
                    var existingUserByUserName = await _userManager.FindByEmailAsync(userDto.UserName);
                    if (existingUserByUserName != null && existingUserByUserName.Id != userDto.Id)
                        return Result.Failure("اسم المستخدم ليس متاح");
                }
                else
                {
                    user.UserName = GenerateDefaultUserNameFromEmailOrNames(userDto);
                    var existingUserByUserName = await _userManager.FindByEmailAsync(userDto.UserName);
                    while (existingUserByUserName != null && existingUserByUserName.Id != userDto.Id)
                    {
                        user.UserName = GenerateDefaultUserNameFromEmailOrNames(userDto);
                        existingUserByUserName = await _userManager.FindByEmailAsync(userDto.UserName);
                    }
                }

                // Update user details
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.UserName = userDto.UserName;
                user.PhoneNumber = userDto.Phone;
                user.ImagePath = userDto.ImagePath;

                // Persist changes
                var updateResult = await _userManager.UpdateAsync(user);

                return updateResult.Succeeded
                    ? Result.Success()
                    : Result.Failure($"فشل في تحديث المستخدم");
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
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
                    return Result.Failure("المستخدم غير موجود");
                }

                var isAdmin = await _userManager.GetRolesAsync(user);

                if (isAdmin.Contains(AppUserRoles.RoleAdmin))
                {
                    return Result.Failure("لا تستطيع ان تنفذ هذه العملية على الادمن");
                }

                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Lockout indefinitely
                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Result.Success() : Result.Failure($"فشلت العملية: "); ;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
               return Result.Failure($"فشلت العملية: "); ;
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

        public async Task<Result> ChangeUserRoleAsync(int userId, string oldRole, string newRole)
        {
            // Validate input
            if (userId <= 0 || oldRole == newRole)
                return Result.Failure("خطأ في المعاملات");

            // Start transaction
            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
                Result.Failure($"فشل في بدء العملية: ");
            try
            {
                // Find the user
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure("المستخدم غير موجود");
                }

                // Remove the user from the old role
                var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
                if (!removeResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    Result.Failure($"فشل في إضافة المستخدم: ");
                }

                // Add the user to the new role
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result.Failure($"فشل في إضافة المستخدم: ");
                }

                // Commit the transaction
                var commitResult = await _unitOfWork.CommitAsync();
                return commitResult.IsSuccess
                    ? Result.Success()
                    : Result.Failure($"فشل في إضافة المستخدم: ");
            }
            catch (Exception ex)
            {
                // Rollback transaction on error
                await _unitOfWork.RollbackAsync();
                return Result.Failure($"فشل في إضافة المستخدم: ");
            }
        }

        public async Task<List<ApplicationRole>?> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return null;
            }

            try
            {
                var roleNames = await _userManager.GetRolesAsync(user);

                var roles = await _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync();

                return roles;
            } 
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<ChangeUserRoleDto?> ChangeUserRoleAndReturnDtoAsync(int userId, string oldRole, string newRole)
        {
            if (userId <= 0 || oldRole == newRole) return null;

            var transactionResult = await _unitOfWork.StartTransactionAsync();
            if (!transactionResult.IsSuccess)
            {
                return null;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
                if (!removeResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var commitResult = await _unitOfWork.CommitAsync();
                if (!commitResult.IsSuccess)
                {
                    return null;
                }

                var roles = await GetUserRolesAsync(userId);

                return new ChangeUserRoleDto
                {
                    Id = userId,
                    oldRole = oldRole,
                    newRole = newRole,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return null;
            }

        }

        public async Task<List<string>?> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return roles;
            } catch (Exception ex)
            {
                return null;
            }

        }
        
        public async Task<List<ApplicationRole>?> GetAllApplicationRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return roles;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Result> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    Result.Failure($" المستخدم غير موجود");
                }

                var result = await _userManager.DeleteAsync(user);

                return 
                    result.Succeeded 
                    ? 
                    Result.Success() 
                    : 
                    Result.Failure($"حدث خطا اثناء حذف المستخدم");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Result.Failure($"حدث خطا اثناء حذف المستخدم");
            }
        }
    }
}
