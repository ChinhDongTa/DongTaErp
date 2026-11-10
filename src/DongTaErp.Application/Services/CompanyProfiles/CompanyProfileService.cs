namespace DongTaErp.Application.Services.CompanyProfiles;

public class CompanyProfileService(
    IUnitOfWork unitOfWork,
    ILogger<CompanyProfileService> logger,
    IValidator<CreateCompanyProfileDto> createValidator,
    IValidator<UpdateCompanyProfileDto> updateValidator) : ICompanyProfileService
{
    public Task<Result<CompanyProfileDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.CompanyProfiles, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<CompanyProfileDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.CompanyProfiles, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateCompanyProfileDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.CompanyProfiles, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateCompanyProfileDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.CompanyProfiles, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.CompanyProfiles, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.CompanyProfiles, unitOfWork, id, logger, ct);

    public Task<Result<UpdateCompanyProfileDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.CompanyProfiles, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<CompanyProfileDto>>> SearchAsync(string? searchTerm,
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
    private static Expression<Func<CompanyProfile, CompanyProfileDto>> ToDto() => Extensions.ExpressionToDto();

    private static CompanyProfile CreateEntity(CreateCompanyProfileDto dto) => new ()
    {
        Name = dto.Name,
        TaxCode = dto.TaxCode,
        Phone = dto.Phone,
        Email = dto.Email,
        Address = dto.Address,
        Currency = dto.Currency
    };

    private static Expression<Func<CompanyProfile, UpdateCompanyProfileDto>> ToUpdateDto() => c => new UpdateCompanyProfileDto
    {
        Id = c.Id,
        Name = c.Name,
        TaxCode = c.TaxCode,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        Currency = c.Currency
    };

    private static void UpdateEntity(CompanyProfile companyProfile, UpdateCompanyProfileDto dto)
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