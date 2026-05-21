using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescription;

public class UpdatePrescriptionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
}
