using Elaaj.Application.Common.Models;
using Elaaj.Application.Features.Pharmacies.Dtos;
using MediatR;

namespace Elaaj.Application.Features.Pharmacies.Queries.SearchPharmacies;

public class SearchPharmaciesQuery : IRequest<PagedResult<PharmacyDto>>
{
    public string? Keyword { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}