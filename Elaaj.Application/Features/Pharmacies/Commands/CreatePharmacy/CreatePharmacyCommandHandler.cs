using AutoMapper;
using Elaaj.Application.Interfaces;
using Elaaj.Application.Interfaces.Services;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, Guid>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext; 
    private readonly UserManager<User> _userManager;
    private readonly IFileService _fileService;

    public CreatePharmacyCommandHandler(
        IGenericRepository<Pharmacy> repository,
        IMapper mapper,
        IUserContext userContext,
        UserManager<User> userManager,
        IFileService fileService)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
        _userManager = userManager;
        _fileService = fileService;
    }

    public async Task<Guid> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        string imageUrl = null;

        if (request.ImageUrl != null) // اتأكد إن الـ Command فيه الحقل ده
        {
            imageUrl = await _fileService.UploadFileAsync(request.ImageUrl, "pharmacies");
        }

        var pharmacy = _mapper.Map<Pharmacy>(request);

        pharmacy.ImageUrl = imageUrl;

        pharmacy.OwnerId = currentUser.Id;

        pharmacy.Admins.Add(new PharmacyAdmin
        {
            UserId = currentUser.Id,
            Role = UserRoles.PharmacyOwner
        });

        await _repository.AddAsync(pharmacy);
        await _repository.SaveChangesAsync();


        var user = await _userManager.FindByIdAsync(currentUser.Id);
        if (user != null)
        {
            var isInRole = await _userManager.IsInRoleAsync(user, UserRoles.PharmacyOwner);
            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.PharmacyOwner);
            }
        }

        return pharmacy.Id;
    }
}
