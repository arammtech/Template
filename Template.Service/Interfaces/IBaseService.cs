namespace Template.Service.Interfaces
{
    public interface IBaseService
    {
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
