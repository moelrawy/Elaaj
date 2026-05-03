using AutoMapper;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Restaurants.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, Guid>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext; 
    private readonly UserManager<User> _userManager;

    public CreatePharmacyCommandHandler(
        IGenericRepository<Pharmacy> repository,
        IMapper mapper,
        IUserContext userContext,
        UserManager<User> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("User context is not available.");

        var pharmacy = _mapper.Map<Pharmacy>(request);
        pharmacy.OwnerId = currentUser.Id;

        pharmacy.Admins.Add(new PharmacyAdmin
        {
            UserId = currentUser.Id,
            PharmacyId = pharmacy.Id,
            Role = UserRoles.PharmacyOwner
        });

        await _repository.AddAsync(pharmacy);
        await _repository.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(currentUser.Id);
        if (user != null && !await _userManager.IsInRoleAsync(user, UserRoles.PharmacyOwner))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.PharmacyOwner);
        }
        return pharmacy.Id;
    }
}
