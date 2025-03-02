using AutoMapper;
using System.Linq.Expressions;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Entities;
using Template.Service.DTOs;
using Template.Service.Interfaces;

namespace Template.Service.Implementations
{
    public class EmployeeService : BaseService
    {
        public EmployeeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

}