namespace DongTaErp.Application.Services.JobPositions;

internal static class Extensions
{
    public static IQueryable<JobPosition> ApplySorting(this IQueryable<JobPosition> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.Created);

        return sortBy.ToLower() switch
        {
            "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "name" => isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "isactive" => isDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<JobPositionDto> ToJobPositionDto(this IQueryable<JobPosition> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<JobPosition, JobPositionDto>> ExpressionToDto() => x => new JobPositionDto
    {
        Id = x.Id,
        Code = x.Code,
        Name = x.Name,
        IsActive = x.IsActive,
        CreatedAt = x.Created
    };
}
