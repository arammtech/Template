using System.Linq.Expressions;
using Template.Domain.Entities;
using Template.Domain.Global;
using Template.Service.DTOs;

namespace Template.Service.Interfaces
{
    public interface IDepartmentService : IBaseService
    {
        DepartmentDto? Get(Expression<Func<Department, bool>> filter);
        Task<DepartmentDto?> GetAsync(Expression<Func<Department, bool>> filter);
        IEnumerable<DepartmentDto> GetAll(Expression<Func<Department, bool>>? filter = null);
        Task<IEnumerable<DepartmentDto>> GetAllAsync(Expression<Func<Department, bool>>? filter = null);
        Result Add(DepartmentDto departmentDto);
        Task<Result> AddAsync(DepartmentDto departmentDto);
        Result AddRange(IEnumerable<DepartmentDto> departmentDtos);
        Task<Result> AddRangeAsync(IEnumerable<DepartmentDto> departmentDtos);
        Result Update(DepartmentDto departmentDto);
        Task<Result> UpdateAsync(DepartmentDto departmentDto);
        Result Delete(int id);
        Task<Result> DeleteAsync(int id);
    }

    public interface IEmployeeService : IBaseService
    {
        EmployeeDto? Get(Expression<Func<Employee, bool>> filter);
        Task<EmployeeDto?> GetAsync(Expression<Func<Employee, bool>> filter);
        IEnumerable<EmployeeDto> GetAll(Expression<Func<Employee, bool>>? filter = null);
        Task<IEnumerable<EmployeeDto>> GetAllAsync(Expression<Func<Employee, bool>>? filter = null);
        void Add(EmployeeDto employeeDto);
        Task AddAsync(EmployeeDto employeeDto);
        void AddRange(IEnumerable<EmployeeDto> employeeDtos);
        Task AddRangeAsync(IEnumerable<EmployeeDto> employeeDtos);
        void Update(int id, EmployeeDto employeeDto);
        Task UpdateAsync(int id, EmployeeDto employeeDto);
        void Delete(int id);
        Task DeleteAsync(int id);
    }
}
