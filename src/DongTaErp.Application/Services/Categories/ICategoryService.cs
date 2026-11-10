namespace DongTaErp.Application.Services.Categories;

public interface ICategoryService
{
    Task<Result<CategoryDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateCategoryDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<CategoryDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<CategoryDto>>> SearchAsync(string? searchTerm,
                                                                   int pageNumber = 1,
                                                                   int pageSize = 20,
                                                                   string? sortBy = null,
                                                                   bool isDescending = false,
                                                                   CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetTypesAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetParentsAsync(CancellationToken ct = default);
}