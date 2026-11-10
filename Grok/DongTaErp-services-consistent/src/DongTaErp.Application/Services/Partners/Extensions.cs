namespace DongTaErp.Application.Services.Partners;

internal static class Extensions
{
    public static IQueryable<Partner> ApplySorting(this IQueryable<Partner> query, string? sortBy = null, bool isDescending = false)
    {
        if(string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(p => p.Created);
        }
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "code" => isDescending ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "type" => isDescending ? query.OrderByDescending(p => p.Type) : query.OrderBy(p => p.Type),
            "phone" => isDescending ? query.OrderByDescending(p => p.Phone) : query.OrderBy(p => p.Phone),
            "email" => isDescending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
            "taxcode" => isDescending ? query.OrderByDescending(p => p.TaxCode) : query.OrderBy(p => p.TaxCode),
            "address" => isDescending ? query.OrderByDescending(p => p.Address) : query.OrderBy(p => p.Address),
            _=> throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }


    public static IQueryable<PartnerDto> ToPartnerDto(this IQueryable<Partner> query)
    {
        return query.Select(ExpressionToDto());
    }
    public static Expression<Func<Partner, PartnerDto>> ExpressionToDto()
    {
        return p => new PartnerDto
        {
            Id = p.Id,
            Address = p.Address,
            Code = p.Code,
            CreatedAt = p.Created,
            Email = p.Email,
            IsActive = p.IsActive,
            Name = p.Name,
            Phone = p.Phone,
            TaxCode = p.TaxCode,
            Type = p.Type
        };
    }
}