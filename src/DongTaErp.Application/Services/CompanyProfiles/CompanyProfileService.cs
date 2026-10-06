namespace DongTaErp.Application.Services.CompanyProfiles;

public class CompanyProfileService : Base.GenericCrudService<CompanyProfile, CompanyProfileDto, CreateCompanyProfileDto, UpdateCompanyProfileDto>, ICompanyProfileService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CompanyProfileService> logger;

    public CompanyProfileService(IUnitOfWork unitOfWork, ILogger<CompanyProfileService> logger) : base(unitOfWork.CompanyProfiles, unitOfWork, logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<PaginatedList<CompanyProfileDto>>> SearchCompanyProfilesAsync(string? searchTerm,
                                                                                            int pageNumber = 1,
                                                                                            int pageSize = 20,
                                                                                            string? sortBy = null,
                                                                                            bool isDescending = false,
                                                                                            CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.CompanyProfiles.Query();

            // Build search query
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
                query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), lowerSearchTerm) ||
                                          (c.TaxCode != null && EF.Functions.Like(c.TaxCode.ToLower(), lowerSearchTerm)) ||
                                          EF.Functions.Like(c.Phone.ToLower(), lowerSearchTerm) ||
                                          (c.Email != null && EF.Functions.Like(c.Email.ToLower(), lowerSearchTerm)) ||
                                          EF.Functions.Like(c.Address.ToLower(), lowerSearchTerm));
            }

            // Apply sorting
            query = sortBy != null ? query.ApplySorting(sortBy, isDescending) : query.ApplySorting();

            // Execute query and return paginated result
            var paginatedList = await query.ToCompanyProfileDto().PaginatedListAsync(pageNumber, pageSize, ct);
            return Result<PaginatedList<CompanyProfileDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching company profiles.");
            return Result<PaginatedList<CompanyProfileDto>>.CriticalError("Internal server error occurred while searching company profiles.");
        }
    }
    protected override Expression<Func<CompanyProfile, CompanyProfileDto>> ToDto() => Extensions.ExpressionToDto();

    protected override CompanyProfile CreateEntity(CreateCompanyProfileDto dto) => new ()
    {
        Name = dto.Name,
        TaxCode = dto.TaxCode,
        Phone = dto.Phone,
        Email = dto.Email,
        Address = dto.Address,
        Currency = dto.Currency
    };

    protected override Expression<Func<CompanyProfile, UpdateCompanyProfileDto>> ToUpdateDto() => c => new UpdateCompanyProfileDto
    {
        Id = c.Id,
        Name = c.Name,
        TaxCode = c.TaxCode,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        Currency = c.Currency
    };

    protected override void UpdateEntity(CompanyProfile companyProfile, UpdateCompanyProfileDto dto)
    {
        // Only update fields if they have changed
        if (dto.Name.HasValueAndIsDifferentFrom(companyProfile.Name))
        {
            companyProfile.Name = dto.Name!;
        }

        if (dto.TaxCode.IsDifferentFrom(companyProfile.TaxCode))
        {
            companyProfile.TaxCode = dto.TaxCode;
        }

        if (dto.Phone.HasValueAndIsDifferentFrom(companyProfile.Phone))
        {
            companyProfile.Phone = dto.Phone!;
        }

        if (dto.Email.IsDifferentFrom(companyProfile.Email))
        {
            companyProfile.Email = dto.Email;
        }

        if (dto.Address.HasValueAndIsDifferentFrom(companyProfile.Address))
        {
            companyProfile.Address = dto.Address!;
        }

        if (dto.Currency.HasValueAndIsDifferentFrom(companyProfile.Currency))
        {
            companyProfile.Currency = dto.Currency!;
        }
    }
}