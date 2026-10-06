namespace DongTaErp.Application.Services.CompanyProfiles;

internal static class Extensions
{
    public static IQueryable<CompanyProfile> ApplySorting(this IQueryable<CompanyProfile> query, string? sortBy = null, bool isDescending = false)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "name" => OrderBy(query, c => c.Name, isDescending),
            "phone" => OrderBy(query, c => c.Phone, isDescending),
            "address" => OrderBy(query, c => c.Address, isDescending),
            "taxcode" => OrderBy(query, c => c.TaxCode, isDescending),
            "email" => OrderBy(query, c => c.Email, isDescending),
            "currency" => OrderBy(query, c => c.Currency, isDescending),
            _ => OrderBy(query, c => c.Created, isDescending)
        };
    }

    private static IOrderedQueryable<CompanyProfile> OrderBy<TKey>(
        IQueryable<CompanyProfile> query,
        Expression<Func<CompanyProfile, TKey>> key,
        bool isDescending)
    {
        return isDescending
            ? query.OrderByDescending(key)
            : query.OrderBy(key);
    }

    public static IQueryable<CompanyProfileDto> ToCompanyProfileDto(this IQueryable<CompanyProfile> query) => query.Select(ExpressionToDto());

    public static Expression<Func<CompanyProfile, CompanyProfileDto>> ExpressionToDto() => c => new CompanyProfileDto
    {
        Id = c.Id,
        Name = c.Name,
        TaxCode = c.TaxCode,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        Currency = c.Currency,
        CreatedAt = c.Created
    };
}