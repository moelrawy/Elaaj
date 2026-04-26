using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Commands.ToggleFavorite;

public class ToggleFavoriteCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
}
