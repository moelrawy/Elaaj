using AutoMapper;
using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetMyPharmacies;

public class GetMyPharmaciesQueryHandler : IRequestHandler<GetMyPharmaciesQuery, IEnumerable<PharmacyDto>>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetMyPharmaciesQueryHandler(IGenericRepository<Pharmacy> repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IEnumerable<PharmacyDto>> Handle(GetMyPharmaciesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null) throw new UnauthorizedAccessException();


        var pharmacies = await _repository.GetAllAsync(p =>
            p.OwnerId == currentUser.Id ||
            p.Admins.Any(a => a.UserId == currentUser.Id));

        var dtos = _mapper.Map<IEnumerable<PharmacyDto>>(pharmacies);

        foreach (var dto in dtos)
        {
            var pharmacy = pharmacies.First(p => p.Id == dto.Id);

            if (pharmacy.OwnerId == currentUser.Id)
            {
                dto.Role = "Owner";
            }
            else
            {
                dto.Role = "Admin";
            }
        }

        return dtos;
    }
}