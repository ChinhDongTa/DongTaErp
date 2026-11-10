namespace DongTaErp.Application.Services.VendorBills;

public interface IVendorBillService : ICrudService<VendorBillDto, CreateVendorBillDto, UpdateVendorBillDto>
{
    Task<Result<PaginatedList<VendorBillDto>>> SearchAsync(SimpleSearch search, bool dueOnly = false, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetVendorBillLinesAsync(Guid purchaseOrderId, CancellationToken ct = default);
    Task<Result<List<OpenDocumentDto>>> GetOpenVendorBillsAsync(Guid partnerId, CancellationToken ct = default);
    Task<Result<OpenDocumentDto?>> GetVendorBillAsync(Guid id, CancellationToken ct = default);
}
