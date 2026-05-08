using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetMyPrescriptions
{
    public class GetMyPrescriptionsQuery : IRequest<PagedResult<MyPrescriptionDto>>
    {
        //public string UserId { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
