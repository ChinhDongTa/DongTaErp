namespace DongTaErp.Application.Services.Payrolls;

public interface IPayrollService
{
    Task<Result<PayrollDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdatePayrollDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<PayrollDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreatePayrollDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdatePayrollDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<PayrollDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
}
