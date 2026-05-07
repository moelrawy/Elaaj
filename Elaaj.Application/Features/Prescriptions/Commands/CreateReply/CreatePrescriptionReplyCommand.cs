using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreateReply;

public class CreatePrescriptionReplyCommand : IRequest<Guid>
{
    public Guid PrescriptionId { get; set; } 

    //public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "يجب تحديد الصيدلية التي ترد باسمها.")]
    public Guid PharmacyId { get; set; }

    [Required(ErrorMessage = "رسالة الرد مطلوبة.")]
    public string Message { get; set; } = string.Empty;

    public decimal? TotalPrice { get; set; }

    public bool IsAvailable { get; set; } = true;
}