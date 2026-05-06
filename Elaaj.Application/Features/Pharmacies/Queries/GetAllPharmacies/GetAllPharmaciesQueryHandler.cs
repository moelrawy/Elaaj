using AutoMapper;
using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Application.Features.Pharmacies.Queries.GetPharmacy;
using Elaaj.Application.Users; // الـ Namespace بتاع الـ UserContext
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Restaurants.Domain.Constants; // الـ Namespace بتاع الـ UserRoles

namespace Elaaj.Application.Features.Pharmacies.Queries.GetAllPharmacies;

public class GetAllPharmaciesQueryHandler : IRequestHandler<GetAllPharmaciesQuery, IEnumerable<PharmacyDto>>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAllPharmaciesQueryHandler(
        IGenericRepository<Pharmacy> repository,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IEnumerable<PharmacyDto>> Handle(GetAllPharmaciesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null) 
            throw new UnauthorizedAccessException();

        var pharmacies = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<PharmacyDto>>(pharmacies);
    }
}