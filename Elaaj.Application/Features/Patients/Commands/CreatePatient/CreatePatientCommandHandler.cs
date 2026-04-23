using AutoMapper;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler(ILogger<CreatePatientCommandHandler> logger,
    IGenericRepository<Patient> repository,
    IMapper mapper) : IRequestHandler<CreatePatientCommand,int>
{
    public async Task<int> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new patient {PatientName}", request.FullName);

        var patient = mapper.Map<Patient>(request);

        await repository.AddAsync(patient);
        await repository.SaveChangesAsync();
        return patient.Id;
    }
}
