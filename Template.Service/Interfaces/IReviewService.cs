using System.Linq.Expressions;
using Template.Domain.Entities;
using Template.Domain.Global;
using Template.Service.DTOs;

namespace Template.Service.Interfaces
{
    public interface IReviewService : IBaseService
    {
        Task<Result> AddAsync(Review departmentDto);
        Task<Result> UpdateAsync(Review departmentDto);
        Task<Result> DeleteAsync(int id);
    }

}
