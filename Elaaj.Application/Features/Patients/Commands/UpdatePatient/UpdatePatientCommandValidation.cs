using FluentValidation;

namespace Elaaj.Application.Features.Patients.Commands.UpdatePatient
{
    public class UpdatePatientCommandValidation : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidation()
        {
            RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 100);
        }
    }
}
