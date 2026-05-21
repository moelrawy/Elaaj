using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.DeletePrescription;

public class DeletePrescriptionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}