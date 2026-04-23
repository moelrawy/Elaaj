using Elaaj.Application.Features.Pharmacies.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetNearbyPharmacies
{
    public class GetNearbyPharmaciesQuery : IRequest<IEnumerable<PharmacyDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusInKm { get; set; } = 5;
    }
}
