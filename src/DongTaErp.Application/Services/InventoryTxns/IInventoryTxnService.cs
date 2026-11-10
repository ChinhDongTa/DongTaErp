namespace DongTaErp.Application.Services.InventoryTxns;

public interface IInventoryTxnService
{
    Task<Result<InventoryTxnDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateInventoryTxnDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<InventoryTxnDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateInventoryTxnDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateInventoryTxnDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<InventoryTxnDto>>> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default);
}