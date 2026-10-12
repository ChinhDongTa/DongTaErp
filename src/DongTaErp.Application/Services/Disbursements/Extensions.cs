namespace DongTaErp.Application.Services.Disbursements;

internal static class Extensions
{
    public static IQueryable<Disbursement> ApplySorting(this IQueryable<Disbursement> query, string? sortBy = null, bool isDescending = false)
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

    public static IQueryable<DisbursementDto> ToDisbursementDto(this IQueryable<Disbursement> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<Disbursement, DisbursementDto>> ExpressionToDto() => x => new DisbursementDto
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
        Allocations = x.Allocations.Select(a => new DisbursementAllocationDto
        {
            Id = a.Id,
            DisbursementId = a.DisbursementId,
            VendorBillId = a.VendorBillId,
            VendorBillNumber = a.VendorBill.Number,
            Amount = a.Amount,
            CreatedAt = a.Created
        }).ToList()
    };
}
