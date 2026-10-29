namespace DongTaErp.Application.Services.SalaryComponents;

public interface ISalaryComponentService : ICrudService<SalaryComponentDto, CreateSalaryComponentDto, UpdateSalaryComponentDto>
{
    Task<Result<PaginatedList<SalaryComponentDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result<List<SalaryComponentDto>>> GetActiveAsync(bool? isFixed = null, CancellationToken ct = default);
}
