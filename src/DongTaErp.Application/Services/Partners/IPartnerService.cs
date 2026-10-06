namespace DongTaErp.Application.Services.Partners;


public interface IPartnerService:ICrudService< PartnerDto, CreatePartnerDto, UpdatePartnerDto>
{
    Task<Result<PaginatedList<PartnerDto>>> SearchPartnersAsync(string? searchTerm,
                                                                int pageNumber = 1,
                                                                int pageSize = 20,
                                                                string? sortBy = null,
                                                                bool isDescending = false,
                                                                CancellationToken ct = default);
   
}
