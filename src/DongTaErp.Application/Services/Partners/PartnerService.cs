namespace DongTaErp.Application.Services.Partners;

public class PartnerService : Base.GenericCrudService<Partner, PartnerDto, CreatePartnerDto, UpdatePartnerDto>, IPartnerService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<PartnerService> logger;

    public PartnerService(IUnitOfWork unitOfWork, ILogger<PartnerService> logger) : base(unitOfWork.Partners, unitOfWork, logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<PaginatedList<PartnerDto>>> SearchPartnersAsync(string? searchTerm,
                                                                             int pageNumber = 1,
                                                                             int pageSize = 20,
                                                                             string? sortBy = null,
                                                                             bool isDescending = false,
                                                                             CancellationToken ct = default)
    {
        try
        {
            
            var query=unitOfWork.Partners.Query();
            //Build search query           
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
                query = query.Where(p => EF.Functions.Like(p.Code.ToLower(), lowerSearchTerm) ||
                                          EF.Functions.Like(p.Name.ToLower(), lowerSearchTerm) ||
                                          (p.Phone != null && EF.Functions.Like(p.Phone.ToLower(), lowerSearchTerm)) ||
                                          (p.Email != null && EF.Functions.Like(p.Email.ToLower(), lowerSearchTerm)));
            }

            //Apply sorting
            query=  query.ApplySorting(sortBy, isDescending); 
            //Execute query and return paginated result
            var paginatedList = await query.ToPartnerDto()
                                           .PaginatedListAsync(pageNumber, pageSize, ct);
            return Result<PaginatedList<PartnerDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching partners.");
            return Result<PaginatedList<PartnerDto>>.CriticalError("Internal server error occurred while searching partners.");
        }
    }

    protected override Expression<Func<Partner, PartnerDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Partner CreateEntity(CreatePartnerDto dto) => new()
    {
        Code = dto.Code,
        Name = dto.Name,
        Type = dto.Type,
        Phone = dto.Phone,
        Email = dto.Email,
        TaxCode = dto.TaxCode,
        Address = dto.Address,
        IsActive = true
    };
    protected override Expression<Func<Partner, UpdatePartnerDto>> ToUpdateDto() => p => new UpdatePartnerDto
    {
        Id = p.Id,
        Code = p.Code,
        Name = p.Name,
        Type = p.Type,
        Phone = p.Phone,
        Email = p.Email,
        TaxCode = p.TaxCode,
        Address = p.Address,
        IsActive = p.IsActive
    };
    protected override void UpdateEntity(Partner partner, UpdatePartnerDto dto)
    {
        if (dto.Code.HasValueAndIsDifferentFrom(partner.Code))
        {
            partner.Code = dto.Code!;
        }
        if (dto.Name.HasValueAndIsDifferentFrom(partner.Name))
        {
            partner.Name = dto.Name!;
        }
        if (dto.Phone.IsDifferentFrom(partner.Phone))
        {
            partner.Phone = dto.Phone;
        }
        if (dto.Email.IsDifferentFrom(partner.Email))
        {
            partner.Email = dto.Email;
        }
        if (dto.TaxCode.IsDifferentFrom(partner.TaxCode))
        {
            partner.TaxCode = dto.TaxCode;
        }
        if (dto.Address.IsDifferentFrom(partner.Address))
        {
            partner.Address = dto.Address;
        }
        if (dto.Type.HasValueAndIsDifferentFrom(partner.Type))
        {
            partner.Type = dto.Type!.Value;
        }
        if (dto.IsActive.HasValueAndIsDifferentFrom(partner.IsActive))
        {
            partner.IsActive = dto.IsActive!.Value;
        }
    }
}