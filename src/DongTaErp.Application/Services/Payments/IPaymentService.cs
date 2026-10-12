namespace DongTaErp.Application.Services.Payments;

public interface IPaymentService : ICrudService<PaymentDto, CreatePaymentDto, UpdatePaymentDto>
{
    Task<Result<PaginatedList<PaymentDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> PostAsync(Guid id, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
