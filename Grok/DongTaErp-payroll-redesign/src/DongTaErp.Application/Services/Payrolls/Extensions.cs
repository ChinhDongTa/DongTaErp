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
            "grossamount" => isDescending ? query.OrderByDescending(x => x.GrossAmount) : query.OrderBy(x => x.GrossAmount),
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
        GrossAmount = p.GrossAmount,
        DeductionAmount = p.DeductionAmount,
        NetAmount = p.NetAmount,
        PaymentStatus = p.PaymentStatus,
        PaymentDate = p.PaymentDate,
        PaymentMethod = p.PaymentMethod,
        ReferenceNumber = p.ReferenceNumber,
        Notes = p.Notes,
        CreatedAt = p.Created,
        Lines = p.Lines.Select(l => new PayrollLineDto
        {
            Id = l.Id,
            SalaryComponentId = l.SalaryComponentId,
            ComponentName = l.SalaryComponent.Name,
            Type = l.SalaryComponent.Type,
            Quantity = l.Quantity,
            Rate = l.Rate,
            Amount = l.Amount,
            Note = l.Note
        }).ToList()
    };

    public static void Recalculate(this Payroll payroll)
    {
        payroll.GrossAmount = payroll.Lines.Where(x => x.SalaryComponent.Type == SalaryComponentType.Earning).Sum(x => x.Amount);
        payroll.DeductionAmount = payroll.Lines.Where(x => x.SalaryComponent.Type == SalaryComponentType.Deduction).Sum(x => x.Amount);
        payroll.NetAmount = payroll.GrossAmount - payroll.DeductionAmount;
    }
}
