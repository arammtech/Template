using AutoMapper;
using Template.Domain.Entities;
using Template.Service.DTOs;

namespace Template.Service.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Employee, EmployeeDto>().ReverseMap();
        }
    }
}
