using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class InvoiceDto : BaseDto
{
    public string Number { get; set; } = null!;
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public Guid SalesOrderId { get; set; }
    public string SalesOrderNumber { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();
    public string Currency { get; set; } = "VND";
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance => Total - PaidAmount;
    public DateTime? IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceLineDto> Lines { get; set; } = [];
}

public class InvoiceLineDto : BaseDto
{
    public Guid InvoiceId { get; set; }
    public Guid SalesOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreateInvoiceLineDto
{
    public Guid SalesOrderLineId { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateInvoiceDto
{
    public required string Number { get; set; }
    public Guid SalesOrderId { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Today;
    public DateTime? DueDate { get; set; }
    public decimal TaxRate { get; set; } = 0.08m;
    public string? Notes { get; set; }
    public List<CreateInvoiceLineDto> Lines { get; set; } = [];
}

public class UpdateInvoiceDto
{
    public Guid Id { get; set; }
    public string? Number { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Notes { get; set; }
}
