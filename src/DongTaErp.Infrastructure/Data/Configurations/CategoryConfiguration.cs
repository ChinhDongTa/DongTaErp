using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        // CategoryType (1-N)
        builder.HasOne(x => x.CategoryType)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.CategoryTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-reference (Parent/Children)
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique Code trong cùng loại Category
        builder.HasIndex(x => new
        {
            x.CategoryTypeId,
            x.Code
        }).IsUnique();
    }
}