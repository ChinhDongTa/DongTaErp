namespace DongTaErp.Application.Services.Payrolls;

public interface IPayrollService : ICrudService<PayrollDto, CreatePayrollDto, UpdatePayrollDto>
{
    Task<Result<PaginatedList<PayrollDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
}
