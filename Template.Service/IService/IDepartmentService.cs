﻿using System.Linq.Expressions;
using Template.Domain.Entities;
using Template.Service.DTOs;

namespace Template.Service.IService
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
