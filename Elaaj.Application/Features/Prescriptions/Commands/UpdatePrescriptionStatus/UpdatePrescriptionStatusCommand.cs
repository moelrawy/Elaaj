using Elaaj.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescriptionStatus;

public class UpdatePrescriptionStatusCommand : IRequest<bool>
{
    public Guid PrescriptionId { get; set; }
    public PrescriptionStatus NewStatus { get; set; }
    public string UserId { get; set; } 
}