namespace DongTaErp.Application.Services.Payrolls;

/// <summary>
/// CRUD + search. Tính thực lãnh đơn giản. Workflow tính lương phức tạp (công thức, thuế, BHXH)
/// nên tách command MediatR/Wolverine khi có quy tắc nghiệp vụ đầy đủ.
/// </summary>
public class PayrollService    : Base.GenericCrudService<Payroll, PayrollDto, CreatePayrollDto, UpdatePayrollDto>,      IPayrollService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PayrollService> _logger;

    public PayrollService(IUnitOfWork unitOfWork, ILogger<PayrollService> logger, IValidator<CreatePayrollDto> createValidator, IValidator<UpdatePayrollDto> updateValidator)
        : base(unitOfWork.Payrolls, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<PayrollDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.Payrolls.Query();

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
            _logger.LogError(ex, "Error searching payrolls");
            return Result<PaginatedList<PayrollDto>>.CriticalError("Internal server error occurred while searching payrolls.");
        }
    }

    protected override Expression<Func<Payroll, PayrollDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Payroll CreateEntity(CreatePayrollDto dto) => new()
    {
        EmployeeId = dto.EmployeeId,
        PeriodStartDate = dto.PeriodStartDate,
        PeriodEndDate = dto.PeriodEndDate,
        BaseSalary = dto.BaseSalary,
        DaysWorked = dto.DaysWorked,
        OvertimeHours = dto.OvertimeHours,
        OvertimeAmount = dto.OvertimeAmount,
        BonusAmount = dto.BonusAmount,
        Deductions = dto.Deductions,
        DeductionDetails = dto.DeductionDetails,
        NetAmount = dto.NetAmount ?? Extensions.CalculateNet(dto.BaseSalary, dto.OvertimeAmount, dto.BonusAmount, dto.Deductions),
        PaymentStatus = dto.PaymentStatus,
        PaymentDate = dto.PaymentDate,
        PaymentMethod = dto.PaymentMethod,
        ReferenceNumber = dto.ReferenceNumber,
        Notes = dto.Notes
    };

    protected override Expression<Func<Payroll, UpdatePayrollDto>> ToUpdateDto() => p => new UpdatePayrollDto
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        PeriodStartDate = p.PeriodStartDate,
        PeriodEndDate = p.PeriodEndDate,
        BaseSalary = p.BaseSalary,
        DaysWorked = p.DaysWorked,
        OvertimeHours = p.OvertimeHours,
        OvertimeAmount = p.OvertimeAmount,
        BonusAmount = p.BonusAmount,
        Deductions = p.Deductions,
        DeductionDetails = p.DeductionDetails,
        NetAmount = p.NetAmount,
        PaymentStatus = p.PaymentStatus,
        PaymentDate = p.PaymentDate,
        PaymentMethod = p.PaymentMethod,
        ReferenceNumber = p.ReferenceNumber,
        Notes = p.Notes
    };

    protected override void UpdateEntity(Payroll entity, UpdatePayrollDto dto)
    {
        if (dto.EmployeeId.HasValueAndIsDifferentFrom(entity.EmployeeId))
            entity.EmployeeId = dto.EmployeeId!.Value;

        if (dto.PeriodStartDate.HasValueAndIsDifferentFrom(entity.PeriodStartDate))
            entity.PeriodStartDate = dto.PeriodStartDate!.Value;

        if (dto.PeriodEndDate.HasValueAndIsDifferentFrom(entity.PeriodEndDate))
            entity.PeriodEndDate = dto.PeriodEndDate!.Value;

        if (dto.BaseSalary.HasValueAndIsDifferentFrom(entity.BaseSalary))
            entity.BaseSalary = dto.BaseSalary!.Value;

        if (dto.DaysWorked.HasValueAndIsDifferentFrom(entity.DaysWorked))
            entity.DaysWorked = dto.DaysWorked!.Value;

        if (dto.OvertimeHours.IsDifferentFrom(entity.OvertimeHours))
            entity.OvertimeHours = dto.OvertimeHours;

        if (dto.OvertimeAmount.IsDifferentFrom(entity.OvertimeAmount))
            entity.OvertimeAmount = dto.OvertimeAmount;

        if (dto.BonusAmount.IsDifferentFrom(entity.BonusAmount))
            entity.BonusAmount = dto.BonusAmount;

        if (dto.Deductions.IsDifferentFrom(entity.Deductions))
            entity.Deductions = dto.Deductions;

        if (dto.DeductionDetails.IsDifferentFrom(entity.DeductionDetails))
            entity.DeductionDetails = dto.DeductionDetails;

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

        entity.NetAmount = dto.NetAmount
            ?? Extensions.CalculateNet(entity.BaseSalary, entity.OvertimeAmount, entity.BonusAmount, entity.Deductions);
    }
}
