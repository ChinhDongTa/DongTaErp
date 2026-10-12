namespace DongTaErp.Application.Services.Payrolls;

internal static class Extensions
{
    public static IQueryable<Payroll> ApplySorting(this IQueryable<Payroll> query, string? sortBy = null, bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(x => x.PeriodStartDate);

        return sortBy.ToLower() switch
        {
            "periodstartdate" => isDescending ? query.OrderByDescending(x => x.PeriodStartDate) : query.OrderBy(x => x.PeriodStartDate),
            "periodenddate" => isDescending ? query.OrderByDescending(x => x.PeriodEndDate) : query.OrderBy(x => x.PeriodEndDate),
            "basesalary" => isDescending ? query.OrderByDescending(x => x.BaseSalary) : query.OrderBy(x => x.BaseSalary),
            "netamount" => isDescending ? query.OrderByDescending(x => x.NetAmount) : query.OrderBy(x => x.NetAmount),
            "paymentstatus" => isDescending ? query.OrderByDescending(x => x.PaymentStatus) : query.OrderBy(x => x.PaymentStatus),
            "paymentdate" => isDescending ? query.OrderByDescending(x => x.PaymentDate) : query.OrderBy(x => x.PaymentDate),
            _ => throw new ArgumentException($"Invalid sort field: {sortBy}")
        };
    }

    public static IQueryable<PayrollDto> ToPayrollDto(this IQueryable<Payroll> query) =>
        query.Select(ExpressionToDto());

    public static Expression<Func<Payroll, PayrollDto>> ExpressionToDto() => p => new PayrollDto
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        EmployeeName = p.Employee != null ? p.Employee.FullName : string.Empty,
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
        Notes = p.Notes,
        CreatedAt = p.Created
    };

    public static decimal CalculateNet(decimal baseSalary, decimal? overtime, decimal? bonus, decimal? deductions) =>
        baseSalary + (overtime ?? 0) + (bonus ?? 0) - (deductions ?? 0);
}
