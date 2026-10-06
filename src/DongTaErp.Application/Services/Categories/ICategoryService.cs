namespace DongTaErp.Application.Services.Categories;

public interface ICategoryService : ICrudService< CategoryDto, CreateCategoryDto, UpdateCategoryDto>
{
    Task<Result<PaginatedList<CategoryDto>>> SearchCategoriesAsync(string? searchTerm,
                                                                   int pageNumber = 1,
                                                                   int pageSize = 20,
                                                                   string? sortBy = null,
                                                                   bool isDescending = false,
                                                                   CancellationToken ct = default);
}