namespace DongTaErp.Application.Services.Invoices;

public interface IInvoiceService : ICrudService<InvoiceDto, CreateInvoiceDto, UpdateInvoiceDto>
{
    Task<Result<PaginatedList<InvoiceDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> IssueAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
