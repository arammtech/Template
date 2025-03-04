using AutoMapper;
using System.Linq.Expressions;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Entities;
using Template.Service.DTOs;

namespace Template.Service.Implementations
{
    public partial class EmployeeService : BaseService
    {
        public EmployeeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }

}