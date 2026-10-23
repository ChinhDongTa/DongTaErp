namespace DongTaErp.Application.Services.Warehouses;

public interface IWarehouseService : ICrudService< WarehouseDto, CreateWarehouseDto, UpdateWarehouseDto>
{
    Task<Result<PaginatedList<WarehouseDto>>> SearchAsync(
    string? searchTerm,
    int pageNumber = 1,
    int pageSize = 20,
    string? sortBy = null,
    bool isDescending = false,
    CancellationToken ct = default);
    Task<Result<List<StockBalanceDto>>> GetStockAsync(Guid? warehouseId = null, bool lowOnly = false, Guid? productId = null, CancellationToken ct = default);
}