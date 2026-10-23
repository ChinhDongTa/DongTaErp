using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.Payments;

public class PaymentService
    : Base.GenericCrudService<Payment, PaymentDto, CreatePaymentDto, UpdatePaymentDto>,
      IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IUnitOfWork unitOfWork,
        ILogger<PaymentService> logger,
        IValidator<CreatePaymentDto> createValidator,
        IValidator<UpdatePaymentDto> updateValidator)
        : base(unitOfWork.Payments, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<PaymentDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.Payments.Query();
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Number.ToLower(), term) ||
                    EF.Functions.Like(x.Partner.Name.ToLower(), term) ||
                    (x.ReferenceNumber != null && EF.Functions.Like(x.ReferenceNumber.ToLower(), term)));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToPaymentDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<PaymentDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching payments");
            return Result<PaginatedList<PaymentDto>>.CriticalError("Internal server error occurred while searching payments.");
        }
    }

    public async Task<Result> PostAsync(Guid id, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.Payments.Query()
            .Include(x => x.Allocations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (payment is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(Payment), id));
        if (payment.Status != CustomerPaymentStatus.Draft)
            return Result.Error("Chỉ ghi sổ phiếu thu đang nháp.");

        var allocated = payment.Allocations.Sum(x => x.Amount);
        if (allocated > payment.Amount)
            return Result.Error("Tổng phân bổ vượt số tiền thu.");

        var invoiceIds = payment.Allocations.Select(x => x.InvoiceId).ToList();
        var invoices = await _unitOfWork.Invoices.Query().Where(x => invoiceIds.Contains(x.Id)).ToListAsync(ct);

        foreach (var allocation in payment.Allocations)
        {
            var invoice = invoices.FirstOrDefault(x => x.Id == allocation.InvoiceId);
            if (invoice is null)
                return Result.Error("Hóa đơn phân bổ không tồn tại.");
            if (invoice.PartnerId != payment.PartnerId)
                return Result.Error("Không phân bổ sang hóa đơn của khách khác.");
            if (invoice.Status is not (InvoiceStatus.Issued or InvoiceStatus.PartiallyPaid))
                return Result.Error("Chỉ phân bổ vào hóa đơn đã phát hành và còn nợ.");
            if (allocation.Amount > invoice.Total - invoice.PaidAmount)
                return Result.Error($"Phân bổ vượt số còn nợ của hóa đơn {invoice.Number}.");
        }

        foreach (var allocation in payment.Allocations)
        {
            var invoice = invoices.First(x => x.Id == allocation.InvoiceId);
            invoice.PaidAmount += allocation.Amount;
            if (invoice.PaidAmount >= invoice.Total)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidAt = DateTime.UtcNow;
            }
            else
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }
        }

        payment.Status = CustomerPaymentStatus.Posted;
        payment.PostedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.Payments.Query()
            .Include(x => x.Allocations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (payment is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(Payment), id));
        if (payment.Status == CustomerPaymentStatus.Cancelled)
            return Result.Error("Phiếu thu đã hủy.");

        if (payment.Status == CustomerPaymentStatus.Posted)
        {
            var invoiceIds = payment.Allocations.Select(x => x.InvoiceId).ToList();
            var invoices = await _unitOfWork.Invoices.Query().Where(x => invoiceIds.Contains(x.Id)).ToListAsync(ct);
            foreach (var allocation in payment.Allocations)
            {
                var invoice = invoices.First(x => x.Id == allocation.InvoiceId);
                invoice.PaidAmount -= allocation.Amount;
                invoice.PaidAt = null;
                invoice.Status = invoice.PaidAmount <= 0 ? InvoiceStatus.Issued : InvoiceStatus.PartiallyPaid;
                if (invoice.PaidAmount < 0)
                    invoice.PaidAmount = 0;
            }
        }

        payment.Status = CustomerPaymentStatus.Cancelled;
        payment.CancelledAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }
    
    protected override Expression<Func<Payment, PaymentDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Payment CreateEntity(CreatePaymentDto dto)
    {
        var entity = new Payment
        {
            Number = dto.Number.Trim(),
            PartnerId = dto.PartnerId,
            PaidAt = dto.PaidAt,
            Amount = dto.Amount,
            Method = dto.Method,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = dto.Notes,
            Status = CustomerPaymentStatus.Draft
        };

        foreach (var allocation in dto.Allocations)
        {
            entity.Allocations.Add(new PaymentAllocation
            {
                InvoiceId = allocation.InvoiceId,
                Amount = allocation.Amount
            });
        }

        return entity;
    }

    protected override Expression<Func<Payment, UpdatePaymentDto>> ToUpdateDto() => x => new UpdatePaymentDto
    {
        Id = x.Id,
        Number = x.Number,
        PaidAt = x.PaidAt,
        Amount = x.Amount,
        Method = x.Method,
        ReferenceNumber = x.ReferenceNumber,
        Notes = x.Notes
    };

    protected override void UpdateEntity(Payment entity, UpdatePaymentDto dto)
    {
        if (entity.Status != CustomerPaymentStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa phiếu thu đang nháp.");

        if (dto.Number.HasValueAndIsDifferentFrom(entity.Number))
            entity.Number = dto.Number!.Trim();
        if (dto.PaidAt.HasValueAndIsDifferentFrom(entity.PaidAt))
            entity.PaidAt = dto.PaidAt!.Value;
        if (dto.Amount.HasValueAndIsDifferentFrom(entity.Amount))
            entity.Amount = dto.Amount!.Value;
        if (dto.Method.HasValueAndIsDifferentFrom(entity.Method))
            entity.Method = dto.Method!.Value;
        if (dto.ReferenceNumber.IsDifferentFrom(entity.ReferenceNumber))
            entity.ReferenceNumber = dto.ReferenceNumber;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;
    }
}
