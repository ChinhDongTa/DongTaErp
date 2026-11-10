namespace DongTaErp.Application.Services.Warehouses;

public interface IWarehouseService
{
    Task<Result<WarehouseDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateWarehouseDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<WarehouseDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateWarehouseDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateWarehouseDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<WarehouseDto>>> SearchAsync(
    string? searchTerm,
    int pageNumber = 1,
    int pageSize = 20,
    string? sortBy = null,
    bool isDescending = false,
    CancellationToken ct = default);
    Task<Result<List<StockBalanceDto>>> GetStockAsync(Guid? warehouseId = null, bool lowOnly = false, Guid? productId = null, CancellationToken ct = default);
}