using Elaaj.Application.Interfaces;
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
    private readonly IFileService _fileService;

    public CreatePrescriptionCommandHandler(IGenericRepository<Prescription> repository,IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }
    public async Task<Guid> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var imageUrl = await _fileService.UploadFileAsync(request.File, "prescriptions");
        var prescription = new Prescription
        {
            UserId = request.UserId,
            ImageUrl = imageUrl,
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