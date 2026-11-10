namespace DongTaErp.Application.Services.Products;

public class ProductService(
    IUnitOfWork unitOfWork,
    ILogger<ProductService> logger,
    IValidator<CreateProductDto> createValidator,
    IValidator<UpdateProductDto> updateValidator) : IProductService
{
    public Task<Result<ProductDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.Products, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<ProductDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.Products, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.Products, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.Products, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.Products, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.Products, unitOfWork, id, logger, ct);

    public Task<Result<UpdateProductDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.Products, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<ProductDto>>> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.Products.Query();

            // Search theo SKU và Name
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";

                query = query.Where(p =>
                    EF.Functions.Like(p.Sku.ToLower(), lowerSearchTerm) ||
                    EF.Functions.Like(p.Name.ToLower(), lowerSearchTerm));
            }

            // Sorting
            query = query.ApplySorting(sortBy, isDescending);
            var paginatedList = await query
                .ToProductDto()
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<ProductDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching products.");

            return Result<PaginatedList<ProductDto>>.CriticalError("Internal server error occurred while searching products.");
        }
    }

    private static Expression<Func<Product, ProductDto>> ToDto() => Extensions.ExpressionToDto();

    private static Product CreateEntity(CreateProductDto dto) => new ()
    {
        Sku = dto.Sku,
        Name = dto.Name,
        CategoryId = dto.CategoryId,
        Unit = dto.Unit,
        CostPrice = dto.CostPrice,
        SalePrice = dto.SalePrice,
        MinStock = dto.MinStock,
        IsActive = dto.IsActive
    };

    private static Expression<Func<Product, UpdateProductDto>> ToUpdateDto() => p => new UpdateProductDto
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name,
        CategoryId = p.CategoryId,
        Unit = p.Unit,
        CostPrice = p.CostPrice,
        SalePrice = p.SalePrice,
        MinStock = p.MinStock,
        IsActive = p.IsActive
    };

    private static void UpdateEntity(Product product, UpdateProductDto dto)
    {
        // Required fields
        if (dto.Sku.HasValueAndIsDifferentFrom(product.Sku))
        {
            product.Sku = dto.Sku!;
        }

        if (dto.Name.HasValueAndIsDifferentFrom(product.Name))
        {
            product.Name = dto.Name!;
        }

        if (dto.Unit.HasValueAndIsDifferentFrom(product.Unit))
        {
            product.Unit = dto.Unit!;
        }

        // Optional fields
        if (dto.CategoryId.IsDifferentFrom(product.CategoryId))
        {
            product.CategoryId = dto.CategoryId;
        }

        if (dto.CostPrice.IsDifferentFrom(product.CostPrice))
        {
            product.CostPrice = dto.CostPrice!.Value;
        }

        if (dto.SalePrice.IsDifferentFrom(product.SalePrice))
        {
            product.SalePrice = dto.SalePrice!.Value;
        }

        if (dto.MinStock.IsDifferentFrom(product.MinStock))
        {
            product.MinStock = dto.MinStock!.Value;
        }

        if (dto.IsActive.IsDifferentFrom(product.IsActive))
        {
            product.IsActive = dto.IsActive;
        }
    }
}