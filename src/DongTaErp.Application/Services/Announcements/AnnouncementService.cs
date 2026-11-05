using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.Announcements;

public class AnnouncementService(IUnitOfWork unitOfWork, IUser user, IAppEmailSender email, ILogger<AnnouncementService> logger) : IAnnouncementService
{
    public async Task<Result<PaginatedList<AnnouncementDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        var query = unitOfWork.Announcements.Query();
        if (!string.IsNullOrWhiteSpace(search.SearchTerm))
        {
            var term = $"%{search.SearchTerm.ToLower()}%";
            query = query.Where(x => EF.Functions.Like(x.Title.ToLower(), term) || EF.Functions.Like(x.Content.ToLower(), term));
        }
        query = query.OrderByDescending(x => x.Created);
        var page = await query.Select(x => new AnnouncementDto
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            Type = x.Type,
            SenderName = x.SenderName,
            Created = x.Created,
            RecipientCount = x.Recipients.Count
        }).PaginatedListAsync(search.PageNumber, search.PageSize, ct);
        return Result<PaginatedList<AnnouncementDto>>.Success(page);
    }

    public async Task<Result<AnnouncementDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var dto = await unitOfWork.Announcements.Query()
            .Where(x => x.Id == id)
            .Select(x => new AnnouncementDto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                Type = x.Type,
                SenderName = x.SenderName,
                Created = x.Created,
                RecipientCount = x.Recipients.Count,
                RecipientNames = x.Recipients.Select(r => r.Employee.FullName).ToList()
            })
            .FirstOrDefaultAsync(ct);
        return dto is null ? Result<AnnouncementDto?>.NotFound("Không tìm thấy thông báo.") : Result<AnnouncementDto?>.Success(dto);
    }

    public async Task<Result<string>> CreateAsync(CreateAnnouncementDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
            return Result<string>.Error("Nhập tiêu đề và nội dung.");

        var employees = unitOfWork.Employees.Query().Where(x => x.Status != EmployeeStatus.Resigned);
        if (dto.SendToAll)
        {
        }
        else if (dto.DepartmentId.HasValue)
            employees = employees.Where(x => x.DepartmentId == dto.DepartmentId);
        else if (dto.EmployeeIds.Count > 0)
            employees = employees.Where(x => dto.EmployeeIds.Contains(x.Id));
        else
            return Result<string>.Error("Chọn người nhận: toàn bộ, một phòng ban, hoặc từng nhân viên.");

        var recipients = await employees.Select(x => new { x.Id, x.FullName, x.Email }).ToListAsync(ct);
        if (recipients.Count == 0)
            return Result<string>.Error("Không có nhân viên nhận.");

        var sender = await unitOfWork.Employees.Query().FirstOrDefaultAsync(x => x.UserId == user.Id, ct);
        var announcement = new Announcement
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Type = dto.Type,
            SenderUserId = user.Id,
            SenderName = sender?.FullName ?? user.Id ?? "Hệ thống",
            Recipients = recipients.Select(x => new AnnouncementRecipient { EmployeeId = x.Id }).ToList()
        };
        await unitOfWork.Announcements.AddAsync(announcement, ct);
        await unitOfWork.SaveChangesAsync(ct);

        if (dto.SendEmail)
        {
            var emails = recipients.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email!).Distinct();
            try
            {
                await email.SendAsync(emails, $"[{dto.Type.ToDisplayName()}] {announcement.Title}", announcement.Content, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to email announcement {Id}", announcement.Id);
            }
        }
        return Result<string>.Created(announcement.Id.ToString());
    }

    public async Task<Result<List<AnnouncementDto>>> GetMineAsync(string userId, CancellationToken ct = default)
    {
        var employeeId = await unitOfWork.Employees.Query().Where(x => x.UserId == userId).Select(x => (Guid?)x.Id).FirstOrDefaultAsync(ct);
        if (employeeId is null)
            return Result<List<AnnouncementDto>>.Success([]);
        var items = await unitOfWork.AnnouncementRecipients.Query()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.Announcement.Created)
            .Select(x => new AnnouncementDto
            {
                Id = x.AnnouncementId,
                Title = x.Announcement.Title,
                Content = x.Announcement.Content,
                Type = x.Announcement.Type,
                SenderName = x.Announcement.SenderName,
                Created = x.Announcement.Created
            })
            .Take(50)
            .ToListAsync(ct);
        return Result<List<AnnouncementDto>>.Success(items);
    }
}
