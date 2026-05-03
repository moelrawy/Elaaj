using Elaaj.Application.Features.Prescriptions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions
{
    public class GetMyPrescriptionsQuery : IRequest<IEnumerable<MyPrescriptionDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
