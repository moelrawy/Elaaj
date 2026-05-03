using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.CreatePrescription;

public class CreatePrescriptionCommand : IRequest<Guid>
{
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "يجب إرفاق صورة الروشتة.")]
    public string ImageUrl { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    [Required(ErrorMessage = "الموقع (خط العرض) مطلوب.")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "الموقع (خط الطول) مطلوب.")]
    public double Longitude { get; set; }
}
