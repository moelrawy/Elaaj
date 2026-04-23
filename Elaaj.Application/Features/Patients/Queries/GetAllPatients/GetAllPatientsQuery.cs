using Elaaj.Application.Features.Patients.Dtos;
using MediatR;

namespace Elaaj.Application.Features.Patients.Queries.GetAllPatients;

public class GetAllPatientsQuery : IRequest<IEnumerable<PatientDto>>
{

}
