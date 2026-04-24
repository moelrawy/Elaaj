using AutoMapper;
using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetNearbyPharmacies;

public class GetNearbyPharmaciesQueryHandler : IRequestHandler<GetNearbyPharmaciesQuery, IEnumerable<PharmacyDto>>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;

    public GetNearbyPharmaciesQueryHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PharmacyDto>> Handle(GetNearbyPharmaciesQuery request, CancellationToken cancellationToken)
    {
        var allpharmacies = await _repository.GetAllAsync();
        var nearbyPharmacies = allpharmacies.Where(p =>
            CalculateDistance(request.Latitude, request.Longitude, p.Latitude, p.Longitude) <= request.RadiusInKm);

        var pharmaciesWithDistance = nearbyPharmacies.Select(p =>
        {
            var dto = _mapper.Map<PharmacyDto>(p);
            dto.Distance = CalculateDistance(request.Latitude, request.Longitude, p.Latitude, p.Longitude);
            return dto;
        }).OrderBy(p => p.Distance);
        return pharmaciesWithDistance;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; 
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double angle) => Math.PI * angle / 180.0;

}

