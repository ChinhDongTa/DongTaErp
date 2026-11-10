namespace DongTaErp.Application.Services.Partners;

public class PartnerService(
    IUnitOfWork unitOfWork,
    ILogger<PartnerService> logger,
    IValidator<CreatePartnerDto> createValidator,
    IValidator<UpdatePartnerDto> updateValidator) : IPartnerService
{
    public Task<Result<PartnerDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.Partners, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<PartnerDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.Partners, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreatePartnerDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.Partners, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdatePartnerDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.Partners, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.Partners, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.Partners, unitOfWork, id, logger, ct);

    public Task<Result<UpdatePartnerDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.Partners, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<PartnerDto>>> SearchAsync(string? searchTerm,
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

    private static Expression<Func<Partner, PartnerDto>> ToDto() => Extensions.ExpressionToDto();

    private static Partner CreateEntity(CreatePartnerDto dto) => new()
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
    private static Expression<Func<Partner, UpdatePartnerDto>> ToUpdateDto() => p => new UpdatePartnerDto
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
    private static void UpdateEntity(Partner partner, UpdatePartnerDto dto)
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
        if (dto.IsActive.IsDifferentFrom(partner.IsActive))
        {
            partner.IsActive = dto.IsActive;
        }
    }
}