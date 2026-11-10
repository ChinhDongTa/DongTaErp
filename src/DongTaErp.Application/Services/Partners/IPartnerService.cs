namespace DongTaErp.Application.Services.Partners;


public interface IPartnerService
{
    Task<Result<PartnerDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdatePartnerDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<PartnerDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreatePartnerDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdatePartnerDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<PartnerDto>>> SearchAsync(string? searchTerm,
                                                                int pageNumber = 1,
                                                                int pageSize = 20,
                                                                string? sortBy = null,
                                                                bool isDescending = false,
                                                                CancellationToken ct = default);
   
}
