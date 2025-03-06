using AutoMapper;
using Template.Domain.Entities;
using Template.Domain.Identity;
using Template.Service.DTOs;
using Template.Service.DTOs.Admin;

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
