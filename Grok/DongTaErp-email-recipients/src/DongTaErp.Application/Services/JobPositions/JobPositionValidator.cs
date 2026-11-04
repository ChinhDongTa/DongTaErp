namespace DongTaErp.Application.Services.JobPositions;

public class CreateJobPositionValidator : AbstractValidator<CreateJobPositionDto>
{
    public CreateJobPositionValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
    }
}

public class UpdateJobPositionValidator : AbstractValidator<UpdateJobPositionDto>
{
    public UpdateJobPositionValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).MaximumLength(30).When(x => x.Code != null);
        RuleFor(x => x.Name).MaximumLength(160).When(x => x.Name != null);
    }
}
