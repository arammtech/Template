using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Template.Domain.Common.IUnitOfWork;
using Template.Domain.Entities;
using Template.Domain.Global;
using Template.Service.DTOs;
using Template.Service.Interfaces;

namespace Template.Service.Implementations
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILog _logger;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, ILog logger) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }

        public DepartmentDto? Get(Expression<Func<Department, bool>> filter)
        {
            var entity = _unitOfWork.Repository<Department>().Get(filter);
            return entity is not null ? _mapper.Map<DepartmentDto>(entity) : null;
        }

        public async Task<DepartmentDto?> GetAsync(Expression<Func<Department, bool>> filter)
        {
            var entity = await _unitOfWork.Repository<Department>().GetAsync(filter);
            return entity is not null ? _mapper.Map<DepartmentDto>(entity) : null;
        }

        public IEnumerable<DepartmentDto> GetAll(Expression<Func<Department, bool>>? filter = null)
        {
            var entities = _unitOfWork.Repository<Department>().GetAll(filter);
            return _mapper.Map<IEnumerable<DepartmentDto>>(entities);
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync(Expression<Func<Department, bool>>? filter = null)
        {
            var entities = await _unitOfWork.Repository<Department>().GetAllAsync(filter);
            return _mapper.Map<IEnumerable<DepartmentDto>>(entities);
        }

        public Result Add(DepartmentDto departmentDto)
        {
            if (departmentDto is null) return Result.Failure("DepartmentDto cannot be null");

            if (string.IsNullOrWhiteSpace(departmentDto.Name))
                return Result.Failure("Department name is required");

            if (departmentDto.Name.Length > 255)
                return Result.Failure("Department name cannot exceed 255 characters");

            var department = _mapper.Map<Department>(departmentDto);

            try
            {
                _unitOfWork.Repository<Department>().Add(department);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to add department: {ex.Message}");
            }
        }


        public async Task<Result> AddAsync(DepartmentDto departmentDto)
        {
            if (departmentDto is null) return Result.Failure("DepartmentDto cannot be null");

            if (string.IsNullOrWhiteSpace(departmentDto.Name))
                return Result.Failure("Department name is required");

            if (departmentDto.Name.Length > 255)
                return Result.Failure("Department name cannot exceed 255 characters");

            var department = _mapper.Map<Department>(departmentDto);

            try
            {
                await _unitOfWork.Repository<Department>().AddAsync(department);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to add department asynchronously: {ex.Message}");
            }
        }

        public Result AddRange(IEnumerable<DepartmentDto> departmentDtos)
        {
            if (departmentDtos == null || !departmentDtos.Any())
                return Result.Failure("No departments provided");

            foreach (var departmentDto in departmentDtos)
            {
                if (string.IsNullOrWhiteSpace(departmentDto.Name))
                    return Result.Failure("One or more departments have an empty name");

                if (departmentDto.Name.Length > 255)
                    return Result.Failure("One or more department names exceed 255 characters");
            }

            var departments = _mapper.Map<IEnumerable<Department>>(departmentDtos);

            try
            {
                _unitOfWork.Repository<Department>().AddRange(departments);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to add departments: {ex.Message}");
            }
        }

        public async Task<Result> AddRangeAsync(IEnumerable<DepartmentDto> departmentDtos)
        {
            if (departmentDtos == null || !departmentDtos.Any())
                return Result.Failure("No departments provided");

            foreach (var departmentDto in departmentDtos)
            {
                if (string.IsNullOrWhiteSpace(departmentDto.Name))
                    return Result.Failure("One or more departments have an empty name");

                if (departmentDto.Name.Length > 255)
                    return Result.Failure("One or more department names exceed 255 characters");
            }

            var departments = _mapper.Map<IEnumerable<Department>>(departmentDtos);

            try
            {
                await _unitOfWork.Repository<Department>().AddRangeAsync(departments);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to add departments asynchronously: {ex.Message}");
            }
        }

        public Result Update(DepartmentDto departmentDto)
        {
            if (departmentDto is null) return Result.Failure("DepartmentDto cannot be null");

            if (departmentDto.Id <= 0) return Result.Failure("Department ID must be greater than 0");

            if (string.IsNullOrWhiteSpace(departmentDto.Name))
                return Result.Failure("Department name is required");

            if (departmentDto.Name.Length > 255)
                return Result.Failure("Department name cannot exceed 255 characters");

            var entity = _unitOfWork.Repository<Department>().Get(d => d.Id == departmentDto.Id);

            if (entity is null) return Result.Failure("Department not found");

            _mapper.Map(departmentDto, entity);

            try
            {
                _unitOfWork.Repository<Department>().Update(entity);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to update department: {ex.Message}");
            }
        }

        public async Task<Result> UpdateAsync(DepartmentDto departmentDto)
        {
            if (departmentDto is null) return Result.Failure("DepartmentDto cannot be null");

            if (departmentDto.Id <= 0) return Result.Failure("Department ID must be greater than 0");

            if (string.IsNullOrWhiteSpace(departmentDto.Name))
                return Result.Failure("Department name is required");

            if (departmentDto.Name.Length > 255)
                return Result.Failure("Department name cannot exceed 255 characters");

            var entity = await _unitOfWork.Repository<Department>().GetAsync(d => d.Id == departmentDto.Id);

            if (entity is null) return Result.Failure("Department not found");

            _mapper.Map(departmentDto, entity);

            try
            {
                _unitOfWork.Repository<Department>().Update(entity);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to update department asynchronously: {ex.Message}");
            }
        }


        public Result Delete(int id)
        {
            if (id <= 0) return Result.Failure("Invalid department ID");

            var entity = _unitOfWork.Repository<Department>().Get(d => d.Id == id);

            if (entity is null) return Result.Failure("Department not found");

            try
            {
                _unitOfWork.Repository<Department>().Delete(entity);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to delete department: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (id <= 0) return Result.Failure("Invalid department ID");

            var entity = await _unitOfWork.Repository<Department>().GetAsync(d => d.Id == id);

            if (entity is null) return Result.Failure("Department not found");

            try
            {
                _unitOfWork.Repository<Department>().Delete(entity);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to delete department asynchronously: {ex.Message}");
            }
        }
    }
}
