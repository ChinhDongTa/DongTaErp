using System.Linq.Expressions;

namespace DongTaErp.Application.Services.Categories;

internal static class Extensions
{
    public static IQueryable<Category> ApplySorting(this IQueryable<Category> query, string? sortBy = null, bool isDescending = false)
    {
        if(string.IsNullOrWhiteSpace(sortBy))
        {
             return query.OrderByDescending(p => p.Created);

        }
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "code" => isDescending ? query.OrderByDescending(c => c.Code) : query.OrderBy(c => c.Code),
            "name" => isDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            _ => throw new ArgumentException($"Invalid sortBy value: {sortBy}")
        };
    }

 

    public static IQueryable<CategoryDto> ToCategoryDto(this IQueryable<Category> query)
    {
        return query.Select(ExpressionToDto());
    }
    public static Expression<Func<Category, CategoryDto>> ExpressionToDto()
    {
        return c => new CategoryDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            CategoryType = c.CategoryType.Name,
            ParentName = c.Parent != null ? c.Parent.Name : null,
            CreatedAt = c.Created
        };
    }
}
