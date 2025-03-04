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


               CreateMap<ApplicationUser, UserDto>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
        .ForMember(dest => dest.Role, opt => opt.Ignore()) // Assuming roles are managed separately
        .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.LockoutEnd != null && src.LockoutEnd > DateTimeOffset.UtcNow))
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.Substring(0,src.Name.IndexOf(' ') - 1)))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.Substring(src.Name.IndexOf(' ') + 1)))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore()); // Lockout management handled separately

        }
    }
}
