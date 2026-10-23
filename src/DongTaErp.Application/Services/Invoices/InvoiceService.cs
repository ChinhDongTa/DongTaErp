using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.Invoices;

public class InvoiceService
    : Base.GenericCrudService<Invoice, InvoiceDto, CreateInvoiceDto, UpdateInvoiceDto>,
      IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IUnitOfWork unitOfWork,
        ILogger<InvoiceService> logger,
        IValidator<CreateInvoiceDto> createValidator,
        IValidator<UpdateInvoiceDto> updateValidator)
        : base(unitOfWork.Invoices, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public override async Task<Result<string>> CreateAsync(CreateInvoiceDto dto, CancellationToken ct = default)
    {
        var number = dto.Number.Trim();
        var exists = await _unitOfWork.Invoices.Query().AnyAsync(x => x.Number == number, ct);
        if (exists)
            return Result<string>.Error($"Số hóa đơn {number} đã tồn tại.");
        return await base.CreateAsync(dto, ct);
    }

    public async Task<Result<PaginatedList<InvoiceDto>>> SearchAsync(SimpleSearch search, string? range = null, bool dueOnly = false, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.Invoices.Query();
            var today = DateTime.Today;
            if (range == "week")
            {
                var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
                query = query.Where(x => x.IssueDate >= weekStart && x.IssueDate <= today);
            }
            else if (range == "month")
            {
                var monthStart = new DateTime(today.Year, today.Month, 1);
                query = query.Where(x => x.IssueDate >= monthStart && x.IssueDate <= today);
            }
            if (dueOnly)
            {
                var dueLimit = today.AddDays(7);
                var open = new[] { InvoiceStatus.Issued, InvoiceStatus.PartiallyPaid };
                query = query.Where(x => open.Contains(x.Status) && x.DueDate != null && x.DueDate <= dueLimit && x.Total > x.PaidAmount);
            }
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Number.ToLower(), term) ||
                    EF.Functions.Like(x.Partner.Name.ToLower(), term) ||
                    EF.Functions.Like(x.SalesOrder.Number.ToLower(), term));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToInvoiceDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<InvoiceDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching invoices");
            return Result<PaginatedList<InvoiceDto>>.CriticalError("Internal server error occurred while searching invoices.");
        }
    }

    public async Task<Result> IssueAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await _unitOfWork.Invoices.Query().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(Invoice), id));
        if (invoice.Status != InvoiceStatus.Draft)
            return Result.Error("Chỉ phát hành hóa đơn đang nháp.");
        if (invoice.Lines.Count == 0)
            return Result.Error("Hóa đơn chưa có dòng.");

        var order = await _unitOfWork.SalesOrders.GetByIdAsync(invoice.SalesOrderId, ct);
        if (order is null || order.Status != SalesOrderStatus.Confirmed)
            return Result.Error("Đơn bán phải đã xác nhận.");
        if (order.PartnerId != invoice.PartnerId)
            return Result.Error("Khách hàng hóa đơn không khớp đơn bán.");

        var lineIds = invoice.Lines.Select(x => x.SalesOrderLineId).ToList();
        var shipped = await _unitOfWork.GoodsIssueLines.Query()
            .Where(x => lineIds.Contains(x.SalesOrderLineId) && x.GoodsIssue.Status == GoodsIssueStatus.Posted)
            .GroupBy(x => x.SalesOrderLineId)
            .Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToListAsync(ct);
        var invoiced = await _unitOfWork.InvoiceLines.Query()
            .Where(x => lineIds.Contains(x.SalesOrderLineId) && x.Invoice.Status != InvoiceStatus.Draft && x.Invoice.Status != InvoiceStatus.Cancelled)
            .GroupBy(x => x.SalesOrderLineId)
            .Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Qty) })
            .ToListAsync(ct);

        foreach (var line in invoice.Lines)
        {
            var shippedQty = shipped.FirstOrDefault(x => x.LineId == line.SalesOrderLineId)?.Qty ?? 0;
            var invoicedQty = invoiced.FirstOrDefault(x => x.LineId == line.SalesOrderLineId)?.Qty ?? 0;
            if (invoicedQty + line.Qty > shippedQty)
                return Result.Error("Số lượng hóa đơn vượt số đã xuất kho.");
        }

        invoice.Status = InvoiceStatus.Issued;
        invoice.IssuedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id, ct);
        if (invoice is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(Invoice), id));
        if (invoice.PaidAmount > 0)
            return Result.Error("Không hủy hóa đơn đã thu tiền. Hủy phân bổ trước.");
        if (invoice.Status == InvoiceStatus.Cancelled)
            return Result.Error("Hóa đơn đã hủy.");

        invoice.Status = InvoiceStatus.Cancelled;
        invoice.CancelledAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result<List<DocumentLineLookupDto>>> GetInvoiceLinesAsync(Guid salesOrderId, CancellationToken ct = default)
    {
        var orderLines = await _unitOfWork.SalesOrderLines.Query().Where(x => x.SalesOrderId == salesOrderId)
            .Select(x => new { x.Id, x.Product.Sku, x.Product.Name, x.Qty, x.UnitPrice }).ToListAsync(ct);
        var ids = orderLines.Select(x => x.Id).ToList();
        var shipped = await _unitOfWork.GoodsIssueLines.Query()
            .Where(x => ids.Contains(x.SalesOrderLineId) && x.GoodsIssue.Status == GoodsIssueStatus.Posted)
            .GroupBy(x => x.SalesOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) }).ToListAsync(ct);
        var invoiced = await _unitOfWork.InvoiceLines.Query()
            .Where(x => ids.Contains(x.SalesOrderLineId) && x.Invoice.Status != InvoiceStatus.Draft && x.Invoice.Status != InvoiceStatus.Cancelled)
            .GroupBy(x => x.SalesOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Qty) }).ToListAsync(ct);
        var lines = orderLines.Select(x =>
        {
            var done = shipped.FirstOrDefault(s => s.LineId == x.Id)?.Qty ?? 0;
            var billed = invoiced.FirstOrDefault(s => s.LineId == x.Id)?.Qty ?? 0;
            return new DocumentLineLookupDto
            {
                LineId = x.Id,
                Label = $"{x.Sku} - {x.Name}",
                OrderedQty = x.Qty,
                DoneQty = done,
                RelatedQty = billed,
                RemainingQty = Math.Max(0, done - billed),
                UnitPrice = x.UnitPrice
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    public async Task<Result<List<OpenDocumentDto>>> GetOpenInvoicesAsync(Guid partnerId, CancellationToken ct = default)
       => Result<List<OpenDocumentDto>>.Success(await _unitOfWork.Invoices.Query()
           .Where(x => x.PartnerId == partnerId && (x.Status == InvoiceStatus.Issued || x.Status == InvoiceStatus.PartiallyPaid))
           .OrderBy(x => x.IssueDate)
           .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
           .ToListAsync(ct));
    public async Task<Result<OpenDocumentDto?>> GetInvoiceAsync(Guid id, CancellationToken ct = default)
      => Result<OpenDocumentDto?>.Success(await _unitOfWork.Invoices.Query().Where(x => x.Id == id)
          .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
          .FirstOrDefaultAsync(ct));
    protected override Expression<Func<Invoice, InvoiceDto>> ToDto() => Extensions.ExpressionToDto();

    protected override Invoice CreateEntity(CreateInvoiceDto dto)
    {
        var order = _unitOfWork.SalesOrders.Query().First(x => x.Id == dto.SalesOrderId);
        var invoice = new Invoice
        {
            Number = dto.Number.Trim(),
            PartnerId = order.PartnerId,
            SalesOrderId = dto.SalesOrderId,
            IssueDate = dto.IssueDate,
            DueDate = dto.DueDate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            Status = InvoiceStatus.Draft
        };

        foreach (var line in dto.Lines)
        {
            var orderLine = _unitOfWork.SalesOrderLines.Query().First(x => x.Id == line.SalesOrderLineId);
            invoice.Lines.Add(new InvoiceLine
            {
                SalesOrderLineId = line.SalesOrderLineId,
                ProductId = orderLine.ProductId,
                Qty = line.Qty,
                UnitPrice = line.UnitPrice,
                LineTotal = line.Qty * line.UnitPrice
            });
        }

        Recalculate(invoice);
        return invoice;
    }

    protected override Expression<Func<Invoice, UpdateInvoiceDto>> ToUpdateDto() => x => new UpdateInvoiceDto
    {
        Id = x.Id,
        Number = x.Number,
        IssueDate = x.IssueDate,
        DueDate = x.DueDate,
        TaxRate = x.TaxRate,
        Notes = x.Notes
    };

    protected override void UpdateEntity(Invoice entity, UpdateInvoiceDto dto)
    {
        if (entity.Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa hóa đơn đang nháp.");

        if (dto.Number.HasValueAndIsDifferentFrom(entity.Number))
            entity.Number = dto.Number!.Trim();
        if (dto.IssueDate.HasValueAndIsDifferentFrom(entity.IssueDate))
            entity.IssueDate = dto.IssueDate!.Value;
        if (dto.DueDate.IsDifferentFrom(entity.DueDate))
            entity.DueDate = dto.DueDate;
        if (dto.TaxRate.HasValueAndIsDifferentFrom(entity.TaxRate))
            entity.TaxRate = dto.TaxRate!.Value;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;

        Recalculate(entity);
    }

    private static void Recalculate(Invoice invoice)
    {
        invoice.SubTotal = invoice.Lines.Sum(x => x.LineTotal);
        invoice.TaxAmount = Math.Round(invoice.SubTotal * invoice.TaxRate, 2);
        invoice.Total = invoice.SubTotal + invoice.TaxAmount;
    }
}
