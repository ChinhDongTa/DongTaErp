namespace DongTaErp.Application.Services.Employees;

internal static class Extensions
{
    public static IQueryable<Employee> ApplySorting(this IQueryable<Employee> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.Created);

        return sortBy.ToLower() switch
        {
            "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "fullname" => isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
            "email" => isDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
            "hiredate" => isDescending ? query.OrderByDescending(x => x.HireDate) : query.OrderBy(x => x.HireDate),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<EmployeeDto> ToEmployeeDto(this IQueryable<Employee> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<Employee, EmployeeDto>> ExpressionToDto() => e => new EmployeeDto
    {
        Id = e.Id,
        Code = e.Code,
        FullName = e.FullName,
        Email = e.Email,
        Phone = e.Phone,
        IdentityNumber = e.IdentityNumber,
        Address = e.Address,
        BirthDate = e.BirthDate,
        HireDate = e.HireDate,
        ResignDate = e.ResignDate,
        Gender = e.Gender.ToString(),
        Status = e.Status.ToString(),
        DepartmentName = e.Department.Name,
        PositionName = e.Position.Name,
        UserName = e.User != null ? e.User.UserName : null,
        CreatedAt = e.Created
    };
}
