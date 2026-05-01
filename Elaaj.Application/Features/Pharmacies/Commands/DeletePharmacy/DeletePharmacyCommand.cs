using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.DeletePharmacy;

public class DeletePharmacyCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public Guid Id { get; set; }
}
