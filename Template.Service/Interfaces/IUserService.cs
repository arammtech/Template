using static Template.Domain.Global.Result;
using Template.Service.DTOs.Admin;
using Template.Domain.Global;

namespace Template.Service.Interfaces
{
    public interface IUserService
    {
        //  Task<PaginatedResult<UserDto>> GetUsersAsync(int page, int pageSize, string role);
        Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize, string role);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<Result> AddUserAsync(UserDto userDto);
        Task<Result> UpdateUserAsync(UserDto userDto);
        Task<Result> DeleteUserAsync(int userId);
    }
}
