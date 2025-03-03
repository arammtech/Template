using System.Linq.Expressions;
using Template.Domain.Entities;
using Template.Service.DTOs;

namespace Template.Service.Interfaces
{
    public interface IDepartmentService : IBaseService
    {
        DepartmentDto? Get(Expression<Func<Department, bool>> filter);
        Task<DepartmentDto?> GetAsync(Expression<Func<Department, bool>> filter);
        IEnumerable<DepartmentDto> GetAll(Expression<Func<Department, bool>>? filter = null);
        Task<IEnumerable<DepartmentDto>> GetAllAsync(Expression<Func<Department, bool>>? filter = null);
        void Add(DepartmentDto departmentDto);
        Task AddAsync(DepartmentDto departmentDto);
        void AddRange(IEnumerable<DepartmentDto> departmentDtos);
        Task AddRangeAsync(IEnumerable<DepartmentDto> departmentDtos);
        void Update(int id, DepartmentDto departmentDto);
        Task UpdateAsync(int id, DepartmentDto departmentDto);
        void Delete(int id);
        Task DeleteAsync(int id);
    }

}
