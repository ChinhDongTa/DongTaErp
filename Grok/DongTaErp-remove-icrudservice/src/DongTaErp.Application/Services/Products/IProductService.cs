namespace DongTaErp.Application.Services.Products;

public interface IProductService
{
    Task<Result<ProductDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateProductDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<ProductDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<ProductDto>>> SearchAsync(
    string? searchTerm,
    int pageNumber = 1,
    int pageSize = 20,
    string? sortBy = null,
    bool isDescending = false,
    CancellationToken ct = default);
}