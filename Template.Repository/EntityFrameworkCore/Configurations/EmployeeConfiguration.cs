using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Entities;

namespace Template.Repository.EntityFrameworkCore.Configurations
{
    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(x=> x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne(x=> x.Department)
                .WithMany(x=> x.Employees)
                .HasForeignKey(x=>x.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
