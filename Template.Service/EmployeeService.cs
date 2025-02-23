using AutoMapper;
using System.Linq.Expressions;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Entities;
using Template.Service.DTOs;
using Template.Service.IService;

namespace Template.Service
{
    public class EmployeeService : BaseService
    {
        public EmployeeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

}