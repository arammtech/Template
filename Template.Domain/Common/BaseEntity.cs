namespace Template.Domain.Common
{
    public class BaseEntity : IEntity<int>
    {
        public int Id { get; set; }
    }
}
