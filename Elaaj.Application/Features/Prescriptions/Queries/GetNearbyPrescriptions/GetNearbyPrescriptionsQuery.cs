using Elaaj.Application.Features.Prescriptions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetNearbyPrescriptions;

public class GetNearbyPrescriptionsQuery : IRequest<IEnumerable<PrescriptionDto>>
{
    public string UserId { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
    public double RadiusInKm { get; set; } = 5;
}
