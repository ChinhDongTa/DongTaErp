namespace DongTaErp.Application.Services.LeaveRequests;

public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequestDto>
{
    public CreateLeaveRequestValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.FromDate).NotEmpty();
        RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate);
        RuleFor(x => x.Days).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(400);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ReviewNote).MaximumLength(400);
    }
}

public class UpdateLeaveRequestValidator : AbstractValidator<UpdateLeaveRequestDto>
{
    public UpdateLeaveRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type.HasValue);
        RuleFor(x => x.Days).GreaterThan(0).When(x => x.Days.HasValue);
        RuleFor(x => x.Reason).MaximumLength(400).When(x => x.Reason != null);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.ReviewNote).MaximumLength(400).When(x => x.ReviewNote != null);
    }
}
