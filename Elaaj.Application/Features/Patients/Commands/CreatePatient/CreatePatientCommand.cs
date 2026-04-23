using MediatR;

namespace Elaaj.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommand : IRequest<int>
{
    public string FullName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
