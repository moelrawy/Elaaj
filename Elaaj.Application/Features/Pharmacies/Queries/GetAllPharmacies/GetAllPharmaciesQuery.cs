using Elaaj.Application.Features.Pharmacies.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetPharmacy;

public class GetAllPharmaciesQuery : IRequest<IEnumerable<PharmacyDto>>
{

}