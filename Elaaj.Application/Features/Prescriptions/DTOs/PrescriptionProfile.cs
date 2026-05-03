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
        CreateMap<Prescription, PrescriptionDto>();
    }
}
