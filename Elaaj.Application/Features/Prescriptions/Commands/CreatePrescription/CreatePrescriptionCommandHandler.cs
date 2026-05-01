using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;

public class CreatePrescriptionCommandHandler : IRequestHandler<CreatePrescriptionCommand, Guid>
{
    private readonly IGenericRepository<Prescription> _repository;

    public CreatePrescriptionCommandHandler(IGenericRepository<Prescription> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var prescription = new Prescription
        {
            UserId = request.UserId,
            ImageUrl = request.ImageUrl,
            Notes = request.Notes,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        await _repository.AddAsync(prescription);
        await _repository.SaveChangesAsync();

        return prescription.Id;
    }
}