namespace Template.Domain.Common.Interfaces
{
    public interface IUserLog : ICreatedAt, IUpdatedAt
    {
        int CreatedBy { get; set; }
        int? UpdatedBy { get; set; }
    }
}
