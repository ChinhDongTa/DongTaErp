namespace DongTaErp.Application.Services.SalesOrders;

public interface ISalesOrderService : ICrudService<SalesOrderDto, CreateSalesOrderDto, UpdateSalesOrderDto>
{
    Task<Result<PaginatedList<SalesOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
