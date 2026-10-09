namespace DongTaErp.Application.Services.Warehouses;

public class WarehouseService : Base.GenericCrudService<Warehouse, WarehouseDto, CreateWarehouseDto, UpdateWarehouseDto>, IWarehouseService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<WarehouseService> logger;

    public WarehouseService(IUnitOfWork unitOfWork, ILogger<WarehouseService> logger, IValidator<CreateWarehouseDto> createValidator, IValidator<UpdateWarehouseDto> updateValidator) : base(unitOfWork.Warehouses, unitOfWork, logger, createValidator, updateValidator)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<PaginatedList<WarehouseDto>>> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.Warehouses.Query();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";

                query = query.Where(w =>
                    EF.Functions.Like(w.Code.ToLower(), lowerSearchTerm) ||
                    EF.Functions.Like(w.Name.ToLower(), lowerSearchTerm) ||
                    (w.City != null && EF.Functions.Like(w.City.ToLower(), lowerSearchTerm)));
            }

            query = sortBy != null
                ? query.ApplySorting(sortBy, isDescending)
                : query.ApplySorting();

            var paginatedList = await query
                .ToWarehouseDto()
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<WarehouseDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching warehouses.");

            return Result<PaginatedList<WarehouseDto>>.CriticalError("Internal server error occurred while searching warehouses.");
        }
    }

    protected override Expression<Func<Warehouse, WarehouseDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Warehouse CreateEntity(CreateWarehouseDto dto) => new()
    {
        Code = dto.Code,
        Name = dto.Name,
        Type = dto.Type,
        CountryCode = dto.CountryCode,
        City = dto.City,
        Address = dto.Address,
        TimeZone = dto.TimeZone,
        IsDefault = dto.IsDefault,
        IsActive = dto.IsActive
    };

    protected override Expression<Func<Warehouse, UpdateWarehouseDto>> ToUpdateDto() => w => new UpdateWarehouseDto
    {
        Id = w.Id,
        Code = w.Code,
        Name = w.Name,
        Type = w.Type,
        CountryCode = w.CountryCode,
        City = w.City,
        Address = w.Address,
        TimeZone = w.TimeZone,
        IsDefault = w.IsDefault,
        IsActive = w.IsActive
    };

    protected override void UpdateEntity(Warehouse warehouse, UpdateWarehouseDto dto)
    {
        // Required fields
        if (dto.Code.HasValueAndIsDifferentFrom(warehouse.Code))
        {
            warehouse.Code = dto.Code!;
        }

        if (dto.Name.HasValueAndIsDifferentFrom(warehouse.Name))
        {
            warehouse.Name = dto.Name!;
        }

        if (dto.CountryCode.HasValueAndIsDifferentFrom(warehouse.CountryCode))
        {
            warehouse.CountryCode = dto.CountryCode!;
        }

        // Optional string fields
        if (dto.City.IsDifferentFrom(warehouse.City))
        {
            warehouse.City = dto.City;
        }

        if (dto.Address.IsDifferentFrom(warehouse.Address))
        {
            warehouse.Address = dto.Address;
        }

        if (dto.TimeZone.IsDifferentFrom(warehouse.TimeZone))
        {
            warehouse.TimeZone = dto.TimeZone;
        }

        // Optional value types
        if (dto.Type.IsDifferentFrom(warehouse.Type))
        {
            warehouse.Type = dto.Type!.Value;
        }

        if (dto.IsDefault.IsDifferentFrom(warehouse.IsDefault))
        {
            warehouse.IsDefault = dto.IsDefault!.Value;
        }

        if (dto.IsActive.IsDifferentFrom(warehouse.IsActive))
        {
            warehouse.IsActive = dto.IsActive!.Value;
        }
    }
}