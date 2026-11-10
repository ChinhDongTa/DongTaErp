namespace DongTaErp.Application.Services.SalesOrders;

public interface ISalesOrderService
{
    Task<Result<SalesOrderDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateSalesOrderDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<SalesOrderDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateSalesOrderDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateSalesOrderDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<SalesOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<NewOrderNoticeDto>>> GetNewOrdersAsync(CancellationToken ct = default);
}
