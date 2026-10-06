using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;
public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasIndex(x => x.Number)
        .IsUnique();

        builder.Property(x => x.SubTotal)
        .HasPrecision(18, 2);

        builder.Property(x => x.TaxRate)
        .HasPrecision(5, 4);

        builder.Property(x => x.TaxAmount)
        .HasPrecision(18, 2);

        builder.Property(x => x.Total)
        .HasPrecision(18, 2);

        builder.HasOne(x => x.Partner)
        .WithMany(p => p.SalesOrders)
        .HasForeignKey(x => x.PartnerId);
        builder.HasOne(x => x.Warehouse).WithMany(w => w.SalesOrders)
            .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}