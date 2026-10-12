using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class PaymentDto : BaseDto
{
    public string Number { get; set; } = null!;
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public DateTime PaidAt { get; set; }
    public decimal Amount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal UnallocatedAmount => Amount - AllocatedAmount;
    public PaymentMethod Method { get; set; }
    public string MethodName => Method.ToDisplayName();
    public CustomerPaymentStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime? PostedAt { get; set; }
    public List<PaymentAllocationDto> Allocations { get; set; } = [];
}

public class PaymentAllocationDto : BaseDto
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public decimal Amount { get; set; }
}

public class CreatePaymentAllocationDto
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
}

public class CreatePaymentDto
{
    public required string Number { get; set; }
    public Guid PartnerId { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public List<CreatePaymentAllocationDto> Allocations { get; set; } = [];
}

public class UpdatePaymentDto
{
    public Guid Id { get; set; }
    public string? Number { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal? Amount { get; set; }
    public PaymentMethod? Method { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
