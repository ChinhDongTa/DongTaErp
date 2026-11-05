namespace DongTaErp.Application.Services.Announcements;

public interface IAnnouncementService
{
    Task<Result<PaginatedList<AnnouncementDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result<AnnouncementDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateAnnouncementDto dto, CancellationToken ct = default);
    Task<Result<List<AnnouncementDto>>> GetMineAsync(string userId, CancellationToken ct = default);
}
