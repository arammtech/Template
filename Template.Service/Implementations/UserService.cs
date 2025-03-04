using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Global;
using Template.Domain.Identity;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Identity;
using Template.Service.DTOs.Admin;
using Template.Service.Interfaces;
using static Template.Domain.Global.Result;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private Dictionary<string, List<ApplicationUser>> _roleUserDictionary;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _roleUserDictionary = new Dictionary<string, List<ApplicationUser>>();
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

    public async Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize, string? role = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return new List<UserDto>();
        }

        if (!_roleUserDictionary.ContainsKey(role))
        {
            await BuildRoleUserDictionary();
        }

        var users = _roleUserDictionary.ContainsKey(role)
            ? _roleUserDictionary[role]
            : new List<ApplicationUser>();

        var paginatedUsers = users
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => _mapper.Map<UserDto>(user));

        return paginatedUsers;
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null ? _mapper.Map<UserDto>(user) : null;
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

            return adminUser != null ? _mapper.Map<UserDto>(adminUser) : null;
        }
        catch (Exception ex)
        {
            // Log the exception (optional)
            return null;
        }
    }

    public async Task<Result> AddUserAsync(UserDto userDto)
    {
        if (userDto == null || string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Email))
        {
            return Result.Failure("Invalid user data");
        }

        try
        {
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user, "DefaultPassword123!"); // Use a default or temporary password

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, userDto.Role[0]); // waiting for mohmmed
                if (!roleResult.Succeeded)
                {
                    return Result.Failure(roleResult.Errors.Select(e => e.Description).FirstOrDefault());
                }

                return Result.Success();
            }

            return Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
        }
        catch (Exception ex)
        {
            // Log the exception (optional)
            return Result.Failure($"Failed to add user: {ex.Message}");
        }
    }

    public async Task<Result> UpdateUserAsync(UserDto userDto)
    {
        if (userDto == null || userDto.Id <= 0)
        {
            return Result.Failure("Invalid user data");
        }

        try
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user == null)
            {
                return Result.Failure("User not found");
            }

            _mapper.Map(userDto, user);
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault());
        }
        catch (Exception ex)
        {
            // Log the exception (optional)
            return Result.Failure($"Failed to update user: {ex.Message}");
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