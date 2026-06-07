using MediatR;
using System;
using System.Collections.Generic;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetAcceptedPrescriptions;

public class GetAcceptedPrescriptionsQuery : IRequest<IEnumerable<AcceptedPrescriptionDto>>
{
    public Guid PharmacyId { get; set; }
}

public record AcceptedPrescriptionDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}