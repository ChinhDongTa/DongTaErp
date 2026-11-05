using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public AnnouncementType Type { get; set; }
    public string TypeName => Type.ToDisplayName();
    public string SenderName { get; set; } = "";
    public DateTimeOffset Created { get; set; }
    public int RecipientCount { get; set; }
    public List<string> RecipientNames { get; set; } = [];
}

public class CreateAnnouncementDto
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public AnnouncementType Type { get; set; } = AnnouncementType.General;
    public Guid? DepartmentId { get; set; }
    public List<Guid> EmployeeIds { get; set; } = [];
    public bool SendToAll { get; set; }
    public bool SendEmail { get; set; } = true;
}
