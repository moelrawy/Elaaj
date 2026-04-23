using FluentValidation;

namespace Elaaj.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandValidation : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidation()
    {
        RuleFor(pt => pt.FullName)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 100);

        RuleFor(pt => pt.Region)
            .NotEmpty().WithMessage("Region is required");
    }
}
