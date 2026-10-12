using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class GoodsIssueConfiguration : IEntityTypeConfiguration<GoodsIssue>
{
    public void Configure(EntityTypeBuilder<GoodsIssue> builder)
    {
        builder.ToTable("GoodsIssues");
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.SalesOrderId, x.WarehouseId });

        builder.HasOne(x => x.SalesOrder)
            .WithMany(o => o.GoodsIssues)
            .HasForeignKey(x => x.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(w => w.GoodsIssues)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class GoodsIssueLineConfiguration : IEntityTypeConfiguration<GoodsIssueLine>
{
    public void Configure(EntityTypeBuilder<GoodsIssueLine> builder)
    {
        builder.ToTable("GoodsIssueLines");
        builder.Property(x => x.Quantity).HasPrecision(18, 3);

        builder.HasOne(x => x.GoodsIssue)
            .WithMany(i => i.Lines)
            .HasForeignKey(x => x.GoodsIssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SalesOrderLine)
            .WithMany(l => l.GoodsIssueLines)
            .HasForeignKey(x => x.SalesOrderLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
