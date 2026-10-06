namespace DongTaErp.Application.Services.CompanyProfiles;

public interface ICompanyProfileService : ICrudService<CompanyProfileDto, CreateCompanyProfileDto, UpdateCompanyProfileDto>
{
    Task<Result<PaginatedList<CompanyProfileDto>>> SearchCompanyProfilesAsync(string? searchTerm,
                                                                              int pageNumber = 1,
                                                                              int pageSize = 20,
                                                                              string? sortBy = null,
                                                                              bool isDescending = false,
                                                                              CancellationToken ct = default);
}
