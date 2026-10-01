using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class InventoryTxnConfiguration : IEntityTypeConfiguration<InventoryTxn>
{
    public void Configure(EntityTypeBuilder<InventoryTxn> builder)
    {
        builder.ToTable("InventoryTxns");

        builder.Property(x => x.QtyChange)
        .HasPrecision(18, 3);

        builder.HasOne(x => x.Product)
        .WithMany(p => p.InventoryTxns)
        .HasForeignKey(x => x.ProductId);
    }
}