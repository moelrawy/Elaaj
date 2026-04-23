using MediatR;

namespace Elaaj.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommand : IRequest
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
