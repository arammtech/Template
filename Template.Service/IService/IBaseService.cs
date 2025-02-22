namespace Template.Service.IService
{
    public interface IBaseService
    {
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
