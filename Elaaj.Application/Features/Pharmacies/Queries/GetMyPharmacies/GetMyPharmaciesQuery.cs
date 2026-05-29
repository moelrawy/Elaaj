using Elaaj.Application.Features.Pharmacies.Dtos;
using MediatR;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetMyPharmacies;

public class GetMyPharmaciesQuery : IRequest<IEnumerable<PharmacyDto>>
{
}