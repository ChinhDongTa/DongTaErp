namespace DongTaErp.Application.Services.Payments;

public interface IPaymentService
{
    Task<Result<PaymentDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdatePaymentDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<PaymentDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdatePaymentDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<PaymentDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
    
}
