using Elaaj.Application.Features.Patients.Dtos;
using MediatR;

namespace Elaaj.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQuery(int id) : IRequest<PatientDto>
{
    public int Id { get; } = id;
}
