using AutoMapper;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Exceptions;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler(ILogger<UpdatePatientCommandHandler> logger,
    IGenericRepository<Patient> repository
    , IMapper mapper) : IRequestHandler<UpdatePatientCommand>
{
    public async Task Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating patient with id: {patientId} with {@Updatedpatient}", request.Id, request);
        var patient = await repository.GetByIdAsync(request.Id);
        if (patient is null)
            throw new NotFoundException(nameof(Patient), request.Id.ToString());

        mapper.Map(request, patient);
        repository.Update(patient);
        await repository.SaveChangesAsync();
        
    }
}
