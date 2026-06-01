using AutoMapper;
using Elaaj.Application.Common.Models;
using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Pharmacies.Queries.SearchPharmacies;

public class SearchPharmaciesQueryHandler : IRequestHandler<SearchPharmaciesQuery, PagedResult<PharmacyDto>>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;

    public SearchPharmaciesQueryHandler(IGenericRepository<Pharmacy> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<PharmacyDto>> Handle(SearchPharmaciesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: string.IsNullOrWhiteSpace(request.Keyword) 
                ? null 
                : p => p.Name.Contains(request.Keyword) || p.Address.Contains(request.Keyword),
            orderBy: q => q.OrderBy(p => p.Name)
        );

        var pharmacyDtos = _mapper.Map<IEnumerable<PharmacyDto>>(items);

        return new PagedResult<PharmacyDto>
        {
            Items = pharmacyDtos.ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}