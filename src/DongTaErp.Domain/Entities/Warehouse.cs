namespace DongTaErp.Domain.Entities;

using DongTaErp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class Warehouse : BaseAuditableEntity
{

    [MaxLength(30)]
    public required string Code { get; set; } 

    [MaxLength(160)]
    public required string Name { get; set; } 

    public WarehouseType Type { get; set; } = WarehouseType.Branch;

    [MaxLength(2)]
    public required string CountryCode { get; set; } = "VN";

    [MaxLength(80)]
    public string? City { get; set; } 

    [MaxLength(300)]
    public string? Address { get; set; } 

    [MaxLength(64)]
    public string? TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StockBalance> StockBalances { get; set; } = [];
    public ICollection<InventoryTxn> InventoryTxns { get; set; } = [];
    public ICollection<SalesOrder> SalesOrders { get; set; } = [];
    public ICollection<GoodsIssue> GoodsIssues { get; set; } = [];
}