using System.Linq.Expressions;
using Template.Domain.Entities;
using Template.Service.DTOs;

namespace Template.Service.Interfaces
{
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
