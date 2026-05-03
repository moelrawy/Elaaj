using Elaaj.Application.Features.Pharmacies.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Queries.GetPharmacyById
{
    public class GetPharmacyByIdQuery : IRequest<PharmacyDto>
    {
        public Guid Id { get; set; }
    }
}
