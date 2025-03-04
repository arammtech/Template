using static Template.Domain.Global.Result;
using Template.Service.DTOs.Admin;

namespace Template.Service.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedResult<UserDto>> GetUsersAsync(int page, int pageSize);
    }
}
