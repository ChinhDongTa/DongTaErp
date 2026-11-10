namespace DongTaErp.Application.Services.SalaryComponents;

public interface ISalaryComponentService
{
    Task<Result<SalaryComponentDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateSalaryComponentDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<SalaryComponentDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateSalaryComponentDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateSalaryComponentDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<SalaryComponentDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result<List<SalaryComponentDto>>> GetActiveAsync(bool? isFixed = null, CancellationToken ct = default);
}
