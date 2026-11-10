using DongTaErp.Application.Services.Base;

namespace DongTaErp.Application.Services.InventoryTxns;

public class InventoryTxnService(
    IUnitOfWork unitOfWork,
    ILogger<InventoryTxnService> logger,
    IValidator<CreateInventoryTxnDto> createValidator,
    IValidator<UpdateInventoryTxnDto> updateValidator) : IInventoryTxnService
{
    public Task<Result<InventoryTxnDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.InventoryTxns, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<InventoryTxnDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.InventoryTxns, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateInventoryTxnDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.InventoryTxns, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateInventoryTxnDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.InventoryTxns, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.InventoryTxns, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.InventoryTxns, unitOfWork, id, logger, ct);

    public Task<Result<UpdateInventoryTxnDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.InventoryTxns, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<InventoryTxnDto>>> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.InventoryTxns.Query();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = $"%{searchTerm.ToLower()}%";

                query = query.Where(x =>
                    EF.Functions.Like(x.Product.Sku.ToLower(), keyword) ||
                    EF.Functions.Like(x.Product.Name.ToLower(), keyword) ||
                    EF.Functions.Like(x.Warehouse.Name.ToLower(), keyword) ||
                    EF.Functions.Like(x.Reference.ToLower(), keyword));
            }

            query = sortBy != null
                ? query.ApplySorting(sortBy, isDescending)
                : query.ApplySorting();

            var paginatedList = await query
                .ToInventoryTxnDto()
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<InventoryTxnDto>>
                .Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching inventory transactions.");

            return Result<PaginatedList<InventoryTxnDto>>.CriticalError("Internal server error occurred while searching inventory transactions.");
        }
    }

    private static Expression<Func<InventoryTxn, InventoryTxnDto>> ToDto() => Extensions.ExpressionToInventoryTxnDto();

    private static InventoryTxn CreateEntity(CreateInventoryTxnDto dto) => new()
    {
        ProductId = dto.ProductId,
        WarehouseId = dto.WarehouseId,
        ContraWarehouseId = dto.ContraWarehouseId,
        Type = dto.Type,
        QtyChange = dto.QtyChange,
        Reference = dto.Reference,
        Note = dto.Note
    };

    private static Expression<Func<InventoryTxn, UpdateInventoryTxnDto>> ToUpdateDto() => x => new UpdateInventoryTxnDto
    {
        Id = x.Id,
        ProductId = x.ProductId,
        WarehouseId = x.WarehouseId,
        ContraWarehouseId = x.ContraWarehouseId,
        Type = x.Type,
        QtyChange = x.QtyChange,
        Reference = x.Reference,
        Note = x.Note
    };

    private static void UpdateEntity(InventoryTxn entity, UpdateInventoryTxnDto dto)
    {
        // Required fields
        if (dto.ProductId.HasValueAndIsDifferentFrom(entity.ProductId))
        {
            entity.ProductId = dto.ProductId!.Value;
        }

        if (dto.WarehouseId.HasValueAndIsDifferentFrom(entity.WarehouseId))
        {
            entity.WarehouseId = dto.WarehouseId!.Value;
        }

        if (dto.Type.HasValueAndIsDifferentFrom(entity.Type))
        {
            entity.Type = dto.Type!.Value;
        }

        if (dto.QtyChange.HasValueAndIsDifferentFrom(entity.QtyChange))
        {
            entity.QtyChange = dto.QtyChange!.Value;
        }

        if (dto.Reference.HasValueAndIsDifferentFrom(entity.Reference))
        {
            entity.Reference = dto.Reference!;
        }

        // Optional fields
        if (dto.ContraWarehouseId.IsDifferentFrom(entity.ContraWarehouseId))
        {
            entity.ContraWarehouseId = dto.ContraWarehouseId;
        }

        if (dto.Note.IsDifferentFrom(entity.Note))
        {
            entity.Note = dto.Note;
        }
    }
}