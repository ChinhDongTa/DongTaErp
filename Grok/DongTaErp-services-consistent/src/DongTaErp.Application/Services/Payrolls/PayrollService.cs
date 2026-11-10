namespace DongTaErp.Application.Services.Payrolls;

/// <summary>
/// CRUD + search. Tính thực lãnh đơn giản. Workflow tính lương phức tạp (công thức, thuế, BHXH)
/// nên tách command MediatR/Wolverine khi có quy tắc nghiệp vụ đầy đủ.
/// </summary>
public class PayrollService(
    IUnitOfWork unitOfWork,
    ILogger<PayrollService> logger,
    IValidator<CreatePayrollDto> createValidator,
    IValidator<UpdatePayrollDto> updateValidator) : IPayrollService
{
    public Task<Result<PayrollDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.Payrolls, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<PayrollDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.Payrolls, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.Payrolls, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.Payrolls, unitOfWork, id, logger, ct);

    public Task<Result<UpdatePayrollDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.Payrolls, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<PayrollDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.Payrolls.Query();

            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(p =>
                    (p.Employee != null && EF.Functions.Like(p.Employee.FullName.ToLower(), term)) ||
                    (p.ReferenceNumber != null && EF.Functions.Like(p.ReferenceNumber.ToLower(), term)) ||
                    (p.Notes != null && EF.Functions.Like(p.Notes.ToLower(), term)));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToPayrollDto()
                .PaginatedListAsync(search.PageNumber, search.PageSize, ct);

            return Result<PaginatedList<PayrollDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching payrolls");
            return Result<PaginatedList<PayrollDto>>.CriticalError("Internal server error occurred while searching payrolls.");
        }
    }

    private static Expression<Func<Payroll, PayrollDto>> ToDto() => Extensions.ExpressionToDto();

    public async Task<Result<string>> CreateAsync(CreatePayrollDto dto, CancellationToken ct = default)
    {
        var errors = await CrudOperations.ValidateAsync(createValidator, dto, ct);
        if (errors.Count > 0)
            return Result<string>.Error([.. errors]);
        var entity = CreateEntity(dto);
        await FillComponentTypesAsync(entity, ct);
        entity.Recalculate();
        await unitOfWork.Payrolls.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result<string>.Created(entity.Id.ToString());
    }

    private async Task FillComponentTypesAsync(Payroll entity, CancellationToken ct)
    {
        var ids = entity.Lines.Select(x => x.SalaryComponentId).ToList();
        var components = await unitOfWork.SalaryComponents.Query().Where(x => ids.Contains(x.Id)).ToListAsync(ct);
        foreach (var line in entity.Lines)
            line.SalaryComponent = components.First(x => x.Id == line.SalaryComponentId);
    }

    private static Payroll CreateEntity(CreatePayrollDto dto)
    {
        var entity = new Payroll
        {
            EmployeeId = dto.EmployeeId,
            PeriodStartDate = dto.PeriodStartDate,
            PeriodEndDate = dto.PeriodEndDate,
            PaymentStatus = dto.PaymentStatus,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = dto.PaymentMethod,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = dto.Notes,
            Lines = [.. dto.Lines.Select(x => new PayrollLine
            {
                SalaryComponentId = x.SalaryComponentId,
                Quantity = x.Quantity,
                Rate = x.Rate,
                Amount = x.Amount == 0 ? x.Quantity * x.Rate : x.Amount,
                Note = x.Note
            })]
        };
        return entity;
    }

    public async Task<Result> UpdateAsync(Guid id, UpdatePayrollDto dto, CancellationToken ct = default)
    {
        var result = await CrudOperations.UpdateAsync(unitOfWork.Payrolls, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);
        if (!result.Succeeded || dto.Lines is null)
            return result;

        var payroll = await unitOfWork.Payrolls.Query().Include(x => x.Lines).FirstAsync(x => x.Id == id, ct);
        foreach (var line in payroll.Lines.ToList())
            unitOfWork.PayrollLines.Delete(line);
        foreach (var line in dto.Lines)
        {
            payroll.Lines.Add(new PayrollLine
            {
                SalaryComponentId = line.SalaryComponentId,
                Quantity = line.Quantity,
                Rate = line.Rate,
                Amount = line.Amount == 0 ? line.Quantity * line.Rate : line.Amount,
                Note = line.Note
            });
        }
        await FillComponentTypesAsync(payroll, ct);
        payroll.Recalculate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    private static Expression<Func<Payroll, UpdatePayrollDto>> ToUpdateDto() => p => new UpdatePayrollDto
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        PeriodStartDate = p.PeriodStartDate,
        PeriodEndDate = p.PeriodEndDate,
        PaymentStatus = p.PaymentStatus,
        PaymentDate = p.PaymentDate,
        PaymentMethod = p.PaymentMethod,
        ReferenceNumber = p.ReferenceNumber,
        Notes = p.Notes
    };

    private static void UpdateEntity(Payroll entity, UpdatePayrollDto dto)
    {
        if (dto.EmployeeId.HasValueAndIsDifferentFrom(entity.EmployeeId))
            entity.EmployeeId = dto.EmployeeId!.Value;
        if (dto.PeriodStartDate.HasValueAndIsDifferentFrom(entity.PeriodStartDate))
            entity.PeriodStartDate = dto.PeriodStartDate!.Value;
        if (dto.PeriodEndDate.HasValueAndIsDifferentFrom(entity.PeriodEndDate))
            entity.PeriodEndDate = dto.PeriodEndDate!.Value;
        if (dto.PaymentStatus.HasValueAndIsDifferentFrom(entity.PaymentStatus))
            entity.PaymentStatus = dto.PaymentStatus!.Value;
        if (dto.PaymentDate.IsDifferentFrom(entity.PaymentDate))
            entity.PaymentDate = dto.PaymentDate;
        if (dto.PaymentMethod.IsDifferentFrom(entity.PaymentMethod))
            entity.PaymentMethod = dto.PaymentMethod;
        if (dto.ReferenceNumber.IsDifferentFrom(entity.ReferenceNumber))
            entity.ReferenceNumber = dto.ReferenceNumber;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;
    }
}
