namespace DongTaErp.Application.Services.VendorBills;

public interface IVendorBillService
{
    Task<Result<VendorBillDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateVendorBillDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<VendorBillDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateVendorBillDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateVendorBillDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<VendorBillDto>>> SearchAsync(SimpleSearch search, bool dueOnly = false, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetVendorBillLinesAsync(Guid purchaseOrderId, CancellationToken ct = default);
    Task<Result<List<OpenDocumentDto>>> GetOpenVendorBillsAsync(Guid partnerId, CancellationToken ct = default);
    Task<Result<OpenDocumentDto?>> GetVendorBillAsync(Guid id, CancellationToken ct = default);
}
