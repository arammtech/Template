using AutoMapper;
using System.Linq.Expressions;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Entities;
using Template.Service.DTOs;
using Template.Service.IService;

namespace Template.Service
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(DepartmentDto departmentDto)
        {
            Department department = _mapper.Map<Department>(departmentDto);
            _unitOfWork.Repository<Department>().Add(department);
        }

        public Task AddAsync(DepartmentDto departmentDto)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<DepartmentDto> departmentDtos)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<DepartmentDto> departmentDtos)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public DepartmentDto? Get(Expression<Func<Department, bool>> filter)
        {
            Department? departmentFromDb = _unitOfWork.Repository<Department>().Get(filter);
            return _mapper.Map<DepartmentDto>(departmentFromDb);
        }

        public IEnumerable<DepartmentDto> GetAll(Expression<Func<Department, bool>>? filter = null)
        {
            IEnumerable<Department> departments = _unitOfWork.Repository<Department>().GetAll(filter);
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public Task<IEnumerable<DepartmentDto>> GetAllAsync(Expression<Func<Department, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<DepartmentDto?> GetAsync(Expression<Func<Department, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, DepartmentDto departmentDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, DepartmentDto departmentDto)
        {
            throw new NotImplementedException();
        }
    }
}
