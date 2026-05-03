using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.UpdatePharmacy;

public class UpdatePharmacyCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string imageUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string WorkingHours { get; set; } = string.Empty;
    public bool HasDelivery { get; set; }
    public string ContactNumber { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
