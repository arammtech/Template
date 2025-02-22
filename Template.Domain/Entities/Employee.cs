using Template.Domain.Common;
using Template.Domain.Common.Interfaces;

namespace Template.Domain.Entities
{
    public class Employee : BaseEntity//, ISoftDeletable, IUserLog
    {
        public string Name { get; set; } = null!;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
        //public int CreatedBy { get; set; }
        //public int? UpdatedBy { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }

        #region Navigation Properties
        public virtual Department Department { get; set; } = null!;
        #endregion
    }
}
