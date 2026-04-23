using AutoMapper;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Exceptions;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandler(ILogger<DeletePatientCommandHandler> logger,
    IGenericRepository<Patient> repository,
    IMapper mapper) : IRequestHandler<DeletePatientCommand>
{
    public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting Patient with id: {patientId}", request.Id);

        var patient = await repository.GetByIdAsync(request.Id);
        if (patient is null)
            throw new NotFoundException(nameof(Patient), request.Id.ToString());

         repository.Delete(patient);
        await repository.SaveChangesAsync();
    }
}
