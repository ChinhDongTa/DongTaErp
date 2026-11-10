namespace DongTaErp.Application.Services.Employees;

internal static class Extensions
{
    public static IQueryable<Employee> ApplySorting(this IQueryable<Employee> query, string? sortBy = null, bool isDescending = false)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "code" => OrderBy(query, e => e.Code, isDescending),
            "fullname" => OrderBy(query, e => e.FullName, isDescending),
            "email" => OrderBy(query, e => e.Email, isDescending),
            "hiredate" => OrderBy(query, e => e.HireDate, isDescending),
            "status" => OrderBy(query, e => e.Status, isDescending),
            "department" => OrderBy(query, e => e.Department.Name, isDescending),
            _ => OrderBy(query, e => e.Created, isDescending)
        };
    }

    private static IOrderedQueryable<Employee> OrderBy<TKey>(
        IQueryable<Employee> query,
        Expression<Func<Employee, TKey>> key,
        bool isDescending)
    {
        return isDescending
            ? query.OrderByDescending(key)
            : query.OrderBy(key);
    }

    public static IQueryable<EmployeeDto> ToEmployeeDto(this IQueryable<Employee> query)
    {
        return query.Select(e => new EmployeeDto
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
        });
    }
}
