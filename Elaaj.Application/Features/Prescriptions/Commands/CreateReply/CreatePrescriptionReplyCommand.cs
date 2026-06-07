using Elaaj.Application.Features.Prescriptions.DTOs;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreateReply;

public class CreatePrescriptionReplyCommand : IRequest<PrescriptionReplyDto> 
{
    public Guid PrescriptionId { get; set; }

    [Required(ErrorMessage = "يجب تحديد الصيدلية التي ترد باسمها.")]
    public Guid PharmacyId { get; set; }

    [Required(ErrorMessage = "رسالة الرد مطلوبة.")]
    public string Message { get; set; } = string.Empty;

    public decimal? TotalPrice { get; set; }

    public bool IsAvailable { get; set; } = true;
}