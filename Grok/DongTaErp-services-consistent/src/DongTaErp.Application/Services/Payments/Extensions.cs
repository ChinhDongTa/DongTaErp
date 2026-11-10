namespace DongTaErp.Application.Services.Payments;

internal static class Extensions
{
    public static IQueryable<Payment> ApplySorting(this IQueryable<Payment> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.PaidAt);

        return sortBy.ToLower() switch
        {
            "number" => isDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
            "paidat" => isDescending ? query.OrderByDescending(x => x.PaidAt) : query.OrderBy(x => x.PaidAt),
            "amount" => isDescending ? query.OrderByDescending(x => x.Amount) : query.OrderBy(x => x.Amount),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<PaymentDto> ToPaymentDto(this IQueryable<Payment> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<Payment, PaymentDto>> ExpressionToDto() => x => new PaymentDto
    {
        Id = x.Id,
        Number = x.Number,
        PartnerId = x.PartnerId,
        PartnerName = x.Partner.Name,
        PaidAt = x.PaidAt,
        Amount = x.Amount,
        AllocatedAmount = x.Allocations.Sum(a => a.Amount),
        Method = x.Method,
        Status = x.Status,
        ReferenceNumber = x.ReferenceNumber,
        Notes = x.Notes,
        PostedAt = x.PostedAt,
        CreatedAt = x.Created,
        Allocations = x.Allocations.Select(a => new PaymentAllocationDto
        {
            Id = a.Id,
            PaymentId = a.PaymentId,
            InvoiceId = a.InvoiceId,
            InvoiceNumber = a.Invoice.Number,
            Amount = a.Amount,
            CreatedAt = a.Created
        }).ToList()
    };
}
