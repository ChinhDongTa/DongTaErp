namespace DongTaErp.Application.Services.Categories;

public class CategoryService : Base.GenericCrudService<Category, CategoryDto, CreateCategoryDto, UpdateCategoryDto>, ICategoryService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CategoryService> logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger):base(unitOfWork.Categories, unitOfWork, logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<PaginatedList<CategoryDto>>> SearchCategoriesAsync(string? searchTerm,
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

    protected override Expression<Func<Category, CategoryDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Category CreateEntity(CreateCategoryDto dto) => new()
    {
        Code = dto.Code,
        Name = dto.Name,
        CategoryTypeId = dto.CategoryTypeId,
        ParentId = dto.ParentId
    };

    protected override Expression<Func<Category, UpdateCategoryDto>> ToUpdateDto() => c => new UpdateCategoryDto
    {
        Id = c.Id,
        Code = c.Code,
        Name = c.Name,
        CategoryTypeId = c.CategoryTypeId,
        ParentId = c.ParentId
    };

    protected override void UpdateEntity(Category category, UpdateCategoryDto dto)
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