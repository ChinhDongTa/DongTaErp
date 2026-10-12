using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class VendorBillDto : BaseDto
{
    public string Number { get; set; } = null!;
    public string? SupplierNumber { get; set; }
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public Guid PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; } = null!;
    public DateTime BillDate { get; set; }
    public DateTime? DueDate { get; set; }
    public VendorBillStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();
    public string Currency { get; set; } = "VND";
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance => Total - PaidAmount;
    public DateTime? PostedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
    public List<VendorBillLineDto> Lines { get; set; } = [];
}

public class VendorBillLineDto : BaseDto
{
    public Guid VendorBillId { get; set; }
    public Guid PurchaseOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreateVendorBillLineDto
{
    public Guid PurchaseOrderLineId { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateVendorBillDto
{
    public required string Number { get; set; }
    public string? SupplierNumber { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public DateTime BillDate { get; set; } = DateTime.Today;
    public DateTime? DueDate { get; set; }
    public decimal TaxRate { get; set; } = 0.08m;
    public string? Notes { get; set; }
    public List<CreateVendorBillLineDto> Lines { get; set; } = [];
}

public class UpdateVendorBillDto
{
    public Guid Id { get; set; }
    public string? Number { get; set; }
    public string? SupplierNumber { get; set; }
    public DateTime? BillDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Notes { get; set; }
}
