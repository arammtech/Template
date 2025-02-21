using Template.Domain.Common;
using Template.Domain.Common.Interfaces;

namespace Template.Domain.Entities
{
    public class Department : BaseEntity, ISoftDeletable, IUserLog
    {
        public string Name { get; set; } = null!;
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        #region Navigation Properties
        public virtual ICollection<Employee> Employees { get; set; } = [];
        #endregion
    }
}
