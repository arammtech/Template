using Template.Domain.Common.IUnitOfWork;
using Template.Service.IService;

namespace Template.Service
{
    public abstract class BaseService : IBaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
