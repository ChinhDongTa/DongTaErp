using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasIndex(x => x.Sku)
        .IsUnique();

        builder.Property(x => x.CostPrice)
        .HasPrecision(18, 2);

        builder.Property(x => x.SalePrice)
        .HasPrecision(18, 2);

        builder.Property(x => x.MinStock)
        .HasPrecision(18, 3);
    }
}