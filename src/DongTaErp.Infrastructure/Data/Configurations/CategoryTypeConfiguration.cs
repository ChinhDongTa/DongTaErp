using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class CategoryTypeConfiguration : IEntityTypeConfiguration<CategoryType>
{
    public void Configure(EntityTypeBuilder<CategoryType> builder)
    {
        builder.ToTable("CategoryTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();
        // Unique Code
        builder.HasIndex(x => x.Code).IsUnique();
    }
}