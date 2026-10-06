using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("StockBalances");
        builder.HasIndex(x => new { x.ProductId, x.WarehouseId }).IsUnique();
        builder.Property(x => x.Qty).HasPrecision(18, 3);
        builder.Property(x => x.MinQty).HasPrecision(18, 3);
        builder.HasOne(x => x.Product).WithMany(p => p.StockBalances).HasForeignKey(x => x.ProductId);
        builder.HasOne(x => x.Warehouse).WithMany(w => w.StockBalances).HasForeignKey(x => x.WarehouseId);
    }
}