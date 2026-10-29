namespace DongTaErp.Application.Services.EmployeeCompensations;

internal static class Extensions
{
    public static IQueryable<EmployeeCompensation> ApplySorting(this IQueryable<EmployeeCompensation> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.EffectiveFrom);
        return sortBy.ToLower() switch
        {
            "employee" => isDescending ? query.OrderByDescending(x => x.Employee.FullName) : query.OrderBy(x => x.Employee.FullName),
            "component" => isDescending ? query.OrderByDescending(x => x.SalaryComponent.Code) : query.OrderBy(x => x.SalaryComponent.Code),
            "amount" => isDescending ? query.OrderByDescending(x => x.Amount) : query.OrderBy(x => x.Amount),
            "effectivefrom" => isDescending ? query.OrderByDescending(x => x.EffectiveFrom) : query.OrderBy(x => x.EffectiveFrom),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<EmployeeCompensationDto> ToEmployeeCompensationDto(this IQueryable<EmployeeCompensation> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<EmployeeCompensation, EmployeeCompensationDto>> ExpressionToDto() => x => new EmployeeCompensationDto
    {
        Id = x.Id,
        EmployeeId = x.EmployeeId,
        EmployeeName = x.Employee.Code + " - " + x.Employee.FullName,
        SalaryComponentId = x.SalaryComponentId,
        ComponentCode = x.SalaryComponent.Code,
        ComponentName = x.SalaryComponent.Name,
        Type = x.SalaryComponent.Type,
        Amount = x.Amount,
        EffectiveFrom = x.EffectiveFrom,
        EffectiveTo = x.EffectiveTo,
        IsActive = x.EffectiveTo == null || x.EffectiveTo >= DateOnly.FromDateTime(DateTime.Today),
        Note = x.Note,
        CreatedAt = x.Created
    };
}
