using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Restaurants.Domain.Constants;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;

public class CreatePrescriptionCommandHandler : IRequestHandler<CreatePrescriptionCommand, Guid>
{
    private readonly IGenericRepository<Prescription> _repository;
    private readonly IFileService _fileService;
    private readonly IUserContext _userContext;

    public CreatePrescriptionCommandHandler(
        IGenericRepository<Prescription> repository,
        IFileService fileService,
        IUserContext userContext)
    {
        _repository = repository;
        _fileService = fileService;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user from Token, not from Request
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Only regular User can send a prescription, not pharmacy staff
        if (currentUser.IsInRole(UserRoles.PharmacyOwner) || currentUser.IsInRole(UserRoles.PharmacyAdmin))
            throw new UnauthorizedAccessException("الصيدلاني لا يمكنه إرسال روشتة");

        // 3. Upload prescription image
        var imageUrl = await _fileService.UploadFileAsync(request.File, "prescriptions");

        // 4. Save prescription to Database with UserId from Token
        var prescription = new Prescription
        {
            UserId = currentUser.Id, // Get UserId from Token, not from Request
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