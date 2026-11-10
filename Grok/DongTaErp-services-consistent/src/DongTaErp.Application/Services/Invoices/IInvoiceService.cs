namespace DongTaErp.Application.Services.Invoices;

public interface IInvoiceService : ICrudService<InvoiceDto, CreateInvoiceDto, UpdateInvoiceDto>
{
    Task<Result<PaginatedList<InvoiceDto>>> SearchAsync(SimpleSearch search, string? range = null, bool dueOnly = false, CancellationToken ct = default);
    Task<Result> IssueAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetInvoiceLinesAsync(Guid salesOrderId, CancellationToken ct = default);
    Task<Result<List<OpenDocumentDto>>> GetOpenInvoicesAsync(Guid partnerId, CancellationToken ct = default);
    Task<Result<OpenDocumentDto?>> GetInvoiceAsync(Guid id, CancellationToken ct = default);
}
