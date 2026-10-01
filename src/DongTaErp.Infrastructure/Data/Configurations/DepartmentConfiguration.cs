using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasIndex(x => x.Code)
        .IsUnique();
    }
}
