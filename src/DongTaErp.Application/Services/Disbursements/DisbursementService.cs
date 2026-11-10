using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.Disbursements;

public class DisbursementService(
    IUnitOfWork unitOfWork,
    ILogger<DisbursementService> logger,
    IValidator<CreateDisbursementDto> createValidator,
    IValidator<UpdateDisbursementDto> updateValidator) : IDisbursementService
{
    public Task<Result<DisbursementDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.Disbursements, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<DisbursementDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.Disbursements, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateDisbursementDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.Disbursements, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateDisbursementDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.Disbursements, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.Disbursements, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.Disbursements, unitOfWork, id, logger, ct);

    public Task<Result<UpdateDisbursementDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.Disbursements, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<DisbursementDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.Disbursements.Query();
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

            var page = await query.ToDisbursementDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<DisbursementDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching disbursements");
            return Result<PaginatedList<DisbursementDto>>.CriticalError("Internal server error occurred while searching disbursements.");
        }
    }

    public async Task<Result> PostAsync(Guid id, CancellationToken ct = default)
    {
        var payment = await unitOfWork.Disbursements.Query()
            .Include(x => x.Allocations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (payment is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(Disbursement), id));
        if (payment.Status != DisbursementStatus.Draft)
            return Result.Error("Chỉ ghi sổ phiếu chi đang nháp.");

        var allocated = payment.Allocations.Sum(x => x.Amount);
        if (allocated > payment.Amount)
            return Result.Error("Tổng phân bổ vượt số tiền chi.");

        var billIds = payment.Allocations.Select(x => x.VendorBillId).ToList();
        var bills = await unitOfWork.VendorBills.Query().Where(x => billIds.Contains(x.Id)).ToListAsync(ct);

        foreach (var allocation in payment.Allocations)
        {
            var bill = bills.FirstOrDefault(x => x.Id == allocation.VendorBillId);
            if (bill is null)
                return Result.Error("Hóa đơn phân bổ không tồn tại.");
            if (bill.PartnerId != payment.PartnerId)
                return Result.Error("Không phân bổ sang hóa đơn của nhà cung cấp khác.");
            if (bill.Status is not (VendorBillStatus.Posted or VendorBillStatus.PartiallyPaid))
                return Result.Error("Chỉ phân bổ vào hóa đơn đã ghi sổ và còn nợ.");
            if (allocation.Amount > bill.Total - bill.PaidAmount)
                return Result.Error($"Phân bổ vượt số còn nợ của hóa đơn {bill.Number}.");
        }

        foreach (var allocation in payment.Allocations)
        {
            var bill = bills.First(x => x.Id == allocation.VendorBillId);
            bill.PaidAmount += allocation.Amount;
            if (bill.PaidAmount >= bill.Total)
            {
                bill.Status = VendorBillStatus.Paid;
                bill.PaidAt = DateTime.UtcNow;
            }
            else
            {
                bill.Status = VendorBillStatus.PartiallyPaid;
            }
        }

        payment.Status = DisbursementStatus.Posted;
        payment.PostedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var payment = await unitOfWork.Disbursements.Query()
            .Include(x => x.Allocations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (payment is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(Disbursement), id));
        if (payment.Status == DisbursementStatus.Cancelled)
            return Result.Error("Phiếu chi đã hủy.");

        if (payment.Status == DisbursementStatus.Posted)
        {
            var billIds = payment.Allocations.Select(x => x.VendorBillId).ToList();
            var bills = await unitOfWork.VendorBills.Query().Where(x => billIds.Contains(x.Id)).ToListAsync(ct);
            foreach (var allocation in payment.Allocations)
            {
                var bill = bills.First(x => x.Id == allocation.VendorBillId);
                bill.PaidAmount -= allocation.Amount;
                bill.PaidAt = null;
                bill.Status = bill.PaidAmount <= 0 ? VendorBillStatus.Posted : VendorBillStatus.PartiallyPaid;
                if (bill.PaidAmount < 0)
                    bill.PaidAmount = 0;
            }
        }

        payment.Status = DisbursementStatus.Cancelled;
        payment.CancelledAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    private static Expression<Func<Disbursement, DisbursementDto>> ToDto() => Extensions.ExpressionToDto();

    private Disbursement CreateEntity(CreateDisbursementDto dto)
    {
        var entity = new Disbursement
        {
            Number = dto.Number.Trim(),
            PartnerId = dto.PartnerId,
            PaidAt = dto.PaidAt,
            Amount = dto.Amount,
            Method = dto.Method,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = dto.Notes,
            Status = DisbursementStatus.Draft
        };

        foreach (var allocation in dto.Allocations)
        {
            entity.Allocations.Add(new DisbursementAllocation
            {
                VendorBillId = allocation.VendorBillId,
                Amount = allocation.Amount
            });
        }

        return entity;
    }

    private static Expression<Func<Disbursement, UpdateDisbursementDto>> ToUpdateDto() => x => new UpdateDisbursementDto
    {
        Id = x.Id,
        Number = x.Number,
        PaidAt = x.PaidAt,
        Amount = x.Amount,
        Method = x.Method,
        ReferenceNumber = x.ReferenceNumber,
        Notes = x.Notes
    };

    private static void UpdateEntity(Disbursement entity, UpdateDisbursementDto dto)
    {
        if (entity.Status != DisbursementStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa phiếu chi đang nháp.");

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
