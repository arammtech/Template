using Template.Domain.Global;

namespace Template.Service.Interfaces
{
    public interface IBaseService
    {
        Task<Result> SaveChangesAsync();
        Result SaveChanges();
    }
}
