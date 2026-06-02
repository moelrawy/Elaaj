using Elaaj.Application.Features.PharmacyAdmins.Dtos;
using MediatR;

namespace Elaaj.Application.Features.PharmacyAdmins.Queries.GetMyPharmacyAdmins;

public class GetMyPharmacyAdminsQuery : IRequest<IEnumerable<PharmacyAdminDto>>
{
    public Guid PharmacyId { get; set; }
}