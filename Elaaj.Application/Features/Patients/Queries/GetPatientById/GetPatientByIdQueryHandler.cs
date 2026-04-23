using AutoMapper;
using Elaaj.Application.Features.Patients.Dtos;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Exceptions;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler(ILogger<GetPatientByIdQueryHandler> logger,
    IGenericRepository<Patient> repository,
    IMapper mapper)
    : IRequestHandler<GetPatientByIdQuery, PatientDto>
{
    public async Task<PatientDto> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Patient {PatientId}", request.Id);

        var patient = await repository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Patient), request.Id.ToString());

        var PatientDto = mapper.Map<PatientDto>(patient);
        return PatientDto;
    }
}
