using AutoMapper;
using Elaaj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.DTOs;

public class PrescriptionProfile : Profile
{
    public PrescriptionProfile()
    {
        // Map Prescription to PrescriptionDto
        CreateMap<Prescription, PrescriptionDto>();

        // Map Prescription to MyPrescriptionDto
        CreateMap<Prescription, MyPrescriptionDto>()
            .ForMember(dest => dest.IsResolved, opt => opt.MapFrom(src => src.Status == Domain.Enums.PrescriptionStatus.Accepted));

        // Map PrescriptionReply to PrescriptionReplyDto
        CreateMap<PrescriptionReply, PrescriptionReplyDto>()
            .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.Pharmacy.Name));
    }
}
