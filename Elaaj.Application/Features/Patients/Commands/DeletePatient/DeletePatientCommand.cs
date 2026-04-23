using Elaaj.Domain.Entities;
using MediatR;

namespace Elaaj.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommand(int id) : IRequest
{
    public int Id { get; } = id;
}
