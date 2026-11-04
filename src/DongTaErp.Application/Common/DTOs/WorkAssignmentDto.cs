namespace DongTaErp.Application.Common.DTOs;

public class WorkRecipientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
}

public class WorkSetupDto
{
    public List<WorkRecipientDto> Positions { get; set; } = [];
    public List<WorkRecipientDto> Employees { get; set; } = [];
    public List<WorkRecipientDto> DutyPositions { get; set; } = [];
    public List<WorkRecipientDto> AssignedEmployees { get; set; } = [];
}