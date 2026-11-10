namespace DongTaErp.Application.Services.InventoryTxns;

public interface IInventoryTxnService : ICrudService<InventoryTxnDto, CreateInventoryTxnDto, UpdateInventoryTxnDto>
{
    Task<Result<PaginatedList<InventoryTxnDto>>> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default);
}