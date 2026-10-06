namespace DongTaErp.Application.Extensions;

public static class QueryableExtensions
{
    public static async Task<Result<TDto?>> SingleOrNotFoundAsync<TDto>(this IQueryable<TDto> query, string entityName, object entityId, CancellationToken ct)
        where TDto : class
    {
        var result = await query.FirstOrDefaultAsync(ct);
        return result == null
            ? Result<TDto?>.NotFound(ErrorHelpers.NotFoundWithId(entityName, entityId))
            : Result<TDto?>.Success(result);
    }

    public static async Task<Result<List<TDto>>> ToListResultAsync<TDto>(this IQueryable<TDto> query, CancellationToken ct)
        where TDto : class
    {
        var items = await query.ToListAsync(ct);
        return Result<List<TDto>>.Success(items);
    }

    public static Result<TDto?> SingleResult<TDto>(TDto? dto, string entityName, string entityId)
    { 
     return dto== null ? (Result<TDto?>.NotFound(entityName,entityId)) : Result<TDto?>.Success(dto);
    }
}