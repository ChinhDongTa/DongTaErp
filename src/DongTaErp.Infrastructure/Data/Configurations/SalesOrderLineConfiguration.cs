using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable("SalesOrderLines");

        builder.Property(x => x.Qty)
        .HasPrecision(18, 3);

        builder.Property(x => x.UnitPrice)
        .HasPrecision(18, 2);

        builder.Property(x => x.LineTotal)
        .HasPrecision(18, 2);

        builder.HasOne(x => x.SalesOrder)
        .WithMany(o => o.Lines)
        .HasForeignKey(x => x.SalesOrderId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
        .WithMany(p => p.SalesLines)
        .HasForeignKey(x => x.ProductId);
    }
}