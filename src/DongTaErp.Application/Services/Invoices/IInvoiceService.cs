namespace DongTaErp.Application.Services.Invoices;

public interface IInvoiceService
{
    Task<Result<InvoiceDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateInvoiceDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<InvoiceDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateInvoiceDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateInvoiceDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<InvoiceDto>>> SearchAsync(SimpleSearch search, string? range = null, bool dueOnly = false, CancellationToken ct = default);
    Task<Result> IssueAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetInvoiceLinesAsync(Guid salesOrderId, CancellationToken ct = default);
    Task<Result<List<OpenDocumentDto>>> GetOpenInvoicesAsync(Guid partnerId, CancellationToken ct = default);
    Task<Result<OpenDocumentDto?>> GetInvoiceAsync(Guid id, CancellationToken ct = default);
}
