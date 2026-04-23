using AutoMapper;
using Elaaj.Application.Features.Patients.Dtos;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Patients.Queries.GetAllPatients;

public class GetAllPatientsQueryHandler(ILogger<GetAllPatientsQueryHandler> logger,
    IGenericRepository<Patient> repository,
    IMapper mapper) : IRequestHandler<GetAllPatientsQuery, IEnumerable<PatientDto>>
{
    public async Task<IEnumerable<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all Patients");

        var patients = await repository.GetAllAsync();

        return mapper.Map<IEnumerable<PatientDto>>(patients);
    }
}