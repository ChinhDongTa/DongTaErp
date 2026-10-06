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
public class GoodsIssueLineConfiguration : IEntityTypeConfiguration<GoodsIssueLine>
{
    public void Configure(EntityTypeBuilder<GoodsIssueLine> builder)
    {
        builder.ToTable("GoodsIssueLines");

        builder.HasOne(x => x.GoodsIssue)
        .WithMany(g => g.Lines)
        .HasForeignKey(x => x.GoodsIssueId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SalesOrderLine)
            .WithMany(x => x.GoodsIssueLines)
            .HasForeignKey(x => x.SalesOrderLineId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
public class GoodsIssueConfiguration : IEntityTypeConfiguration<GoodsIssue>
{
    public void Configure(EntityTypeBuilder<GoodsIssue> builder)
    {
        builder.ToTable("GoodsIssues");
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasOne(x => x.Warehouse)
            .WithMany(w => w.GoodsIssues)
            .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}