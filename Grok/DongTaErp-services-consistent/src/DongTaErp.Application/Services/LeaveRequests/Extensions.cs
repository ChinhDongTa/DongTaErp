namespace DongTaErp.Application.Services.LeaveRequests;

internal static class Extensions
{
    public static IQueryable<LeaveRequest> ApplySorting(this IQueryable<LeaveRequest> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.Created);

        return sortBy.ToLower() switch
        {
            "fromdate" => isDescending ? query.OrderByDescending(x => x.FromDate) : query.OrderBy(x => x.FromDate),
            "todate" => isDescending ? query.OrderByDescending(x => x.ToDate) : query.OrderBy(x => x.ToDate),
            "days" => isDescending ? query.OrderByDescending(x => x.Days) : query.OrderBy(x => x.Days),
            "status" => isDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "type" => isDescending ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<LeaveRequestDto> ToLeaveRequestDto(this IQueryable<LeaveRequest> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<LeaveRequest, LeaveRequestDto>> ExpressionToDto() => x => new LeaveRequestDto
    {
        Id = x.Id,
        EmployeeName = x.Employee.FullName,
        Type = x.Type,
        FromDate = x.FromDate,
        ToDate = x.ToDate,
        Days = x.Days,
        Reason = x.Reason,
        Status = x.Status,
        ReviewNote = x.ReviewNote,
        ReviewedAt = x.ReviewedAt,
        CreatedAt = x.Created
    };
}
