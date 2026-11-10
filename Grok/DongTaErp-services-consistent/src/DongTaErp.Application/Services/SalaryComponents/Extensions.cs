namespace DongTaErp.Application.Services.SalaryComponents;

internal static class Extensions
{
    public static IQueryable<SalaryComponent> ApplySorting(this IQueryable<SalaryComponent> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderBy(x => x.Code);
        return sortBy.ToLower() switch
        {
            "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "name" => isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "type" => isDescending ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
            "isfixed" => isDescending ? query.OrderByDescending(x => x.IsFixed) : query.OrderBy(x => x.IsFixed),
            "isactive" => isDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<SalaryComponentDto> ToSalaryComponentDto(this IQueryable<SalaryComponent> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<SalaryComponent, SalaryComponentDto>> ExpressionToDto() => x => new SalaryComponentDto
    {
        Id = x.Id,
        Code = x.Code,
        Name = x.Name,
        Type = x.Type,
        IsFixed = x.IsFixed,
        IsTaxable = x.IsTaxable,
        IsActive = x.IsActive,
        Note = x.Note,
        CreatedAt = x.Created
    };
}
