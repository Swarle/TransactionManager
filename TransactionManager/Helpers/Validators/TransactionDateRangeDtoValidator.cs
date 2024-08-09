using FluentValidation;
using TransactionManager.Dto;

namespace TransactionManager.Helpers.Validators;

public class TransactionDateRangeDtoValidator : AbstractValidator<TransactionDateRangeDto>
{
    public TransactionDateRangeDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("StartDate is required")
            .Must(HaveNotLocalKind)
            .WithMessage("StartDate must have not offsets");;

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("EndDate is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be greater than StartDate")
            .Must(HaveNotLocalKind)
            .WithMessage("EndDate must have not offsets");
        

        RuleFor(x => x)
            .Must(HaveSameDateTimeKind)
            .WithName("General")
            .WithMessage("StartDate and EndDate must have the same kind");
    }
    
    private static bool HaveSameDateTimeKind(TransactionDateRangeDto dto)
    {
        return dto.StartDate.Kind == dto.EndDate.Kind;
    }

    private static bool HaveNotLocalKind(DateTime date)
    {
        return date.Kind != DateTimeKind.Local;
    }
}