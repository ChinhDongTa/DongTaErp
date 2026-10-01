using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasIndex(x => x.Code)
        .IsUnique();

        builder.HasIndex(x => x.UserId)
        .IsUnique()
        .HasFilter("[UserId] IS NOT NULL");

        builder.HasOne(x => x.Department)
        .WithMany(d => d.Employees)
        .HasForeignKey(x => x.DepartmentId);

        builder.HasOne(x => x.Position)
        .WithMany(p => p.Employees)
        .HasForeignKey(x => x.PositionId);

        builder.HasOne(x => x.User)
        .WithOne(u => u.Employee)
        .HasForeignKey<Employee>(x => x.UserId)
        .OnDelete(DeleteBehavior.SetNull);
    }
}
