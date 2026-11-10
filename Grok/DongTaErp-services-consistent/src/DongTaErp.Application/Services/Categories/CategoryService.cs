namespace DongTaErp.Application.Services.Categories;

public class CategoryService(
    IUnitOfWork unitOfWork,
    ILogger<CategoryService> logger,
    IValidator<CreateCategoryDto> createValidator,
    IValidator<UpdateCategoryDto> updateValidator) : ICategoryService
{
    public Task<Result<CategoryDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.Categories, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<CategoryDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.Categories, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.Categories, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.Categories, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.Categories, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.Categories, unitOfWork, id, logger, ct);

    public Task<Result<UpdateCategoryDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.Categories, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<CategoryDto>>> SearchAsync(string? searchTerm,
                                                                                 int pageNumber = 1,
                                                                                 int pageSize = 20,
                                                                                 string? sortBy = null,
                                                                                 bool isDescending = false,
                                                                                 CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.Categories.Query();

            // Build search query
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
                query = query.Where(c => EF.Functions.Like(c.Code.ToLower(), lowerSearchTerm) ||
                                          EF.Functions.Like(c.Name.ToLower(), lowerSearchTerm));
            }

            // Apply sorting
            query =  query.ApplySorting(sortBy, isDescending);

            // Execute query and return paginated result
            var paginatedList = await query.ToCategoryDto().PaginatedListAsync(pageNumber, pageSize, ct);
            return Result<PaginatedList<CategoryDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching categories.");
            return Result<PaginatedList<CategoryDto>>.CriticalError("Internal server error occurred while searching categories.");
        }
    }

    public async Task<Result<List<LookupItemDto>>> GetTypesAsync(CancellationToken ct = default)
    {
        var items = await unitOfWork.CategoryTypes.Query()
            .OrderBy(x => x.Name)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name })
            .ToListAsync(ct);
        return Result<List<LookupItemDto>>.Success(items);
    }

    public async Task<Result<List<LookupItemDto>>> GetParentsAsync(CancellationToken ct = default)
    {
        var items = await unitOfWork.Categories.Query()
            .OrderBy(x => x.Name)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name })
            .ToListAsync(ct);
        return Result<List<LookupItemDto>>.Success(items);
    }

    private static Expression<Func<Category, CategoryDto>> ToDto() => Extensions.ExpressionToDto();

    private static Category CreateEntity(CreateCategoryDto dto) => new()
    {
        Code = dto.Code,
        Name = dto.Name,
        CategoryTypeId = dto.CategoryTypeId,
        ParentId = dto.ParentId
    };

    private static Expression<Func<Category, UpdateCategoryDto>> ToUpdateDto() => c => new UpdateCategoryDto
    {
        Id = c.Id,
        Code = c.Code,
        Name = c.Name,
        CategoryTypeId = c.CategoryTypeId,
        ParentId = c.ParentId
    };

    private static void UpdateEntity(Category category, UpdateCategoryDto dto)
    {
        //Nếu trường của Entity là required thì dùng HasValueAndIsDifferentFrom.
        if (dto.Code.HasValueAndIsDifferentFrom(category.Code))
        {
            category.Code = dto.Code!;
        }
       
        if (dto.Name.HasValueAndIsDifferentFrom(category.Name))
        {
            category.Name = dto.Name!;
        }
        //Nếu trường của Entity là optional thì dùng IsDifferentFrom.
        if (dto.CategoryTypeId.IsDifferentFrom(category.CategoryTypeId))
        {
            category.CategoryTypeId = dto.CategoryTypeId!.Value;
        }

        if (dto.ParentId.IsDifferentFrom(category.ParentId))
        {
            category.ParentId = dto.ParentId;
        }
    }
}