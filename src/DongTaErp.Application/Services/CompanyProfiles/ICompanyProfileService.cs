namespace DongTaErp.Application.Services.CompanyProfiles;

public interface ICompanyProfileService
{
    Task<Result<CompanyProfileDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateCompanyProfileDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<CompanyProfileDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateCompanyProfileDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateCompanyProfileDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<CompanyProfileDto>>> SearchAsync(string? searchTerm,
                                                                              int pageNumber = 1,
                                                                              int pageSize = 20,
                                                                              string? sortBy = null,
                                                                              bool isDescending = false,
                                                                              CancellationToken ct = default);
}
