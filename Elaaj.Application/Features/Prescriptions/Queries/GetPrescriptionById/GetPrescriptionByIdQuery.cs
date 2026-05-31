using Elaaj.Application.Features.Prescriptions.DTOs;
using MediatR;
using System;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetPrescriptionById;

public class GetPrescriptionByIdQuery : IRequest<MyPrescriptionDto>
{
    public Guid Id { get; set; }
}