namespace DongTaErp.Application.Services.Products;

public interface IProductService : ICrudService< ProductDto, CreateProductDto, UpdateProductDto>
{
    Task<Result<PaginatedList<ProductDto>>> SearchAsync(
    string? searchTerm,
    int pageNumber = 1,
    int pageSize = 20,
    string? sortBy = null,
    bool isDescending = false,
    CancellationToken ct = default);
}