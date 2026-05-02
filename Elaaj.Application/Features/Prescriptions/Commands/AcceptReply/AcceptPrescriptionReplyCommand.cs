using MediatR;
using System;

namespace Elaaj.Application.Features.Prescriptions.Commands.AcceptReply;

public class AcceptPrescriptionReplyCommand : IRequest<bool>
{
    public Guid PrescriptionId { get; set; }
    public Guid ReplyId { get; set; }
    public string UserId { get; set; } = string.Empty;
}