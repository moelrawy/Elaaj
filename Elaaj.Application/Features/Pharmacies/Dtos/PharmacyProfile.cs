using AutoMapper;
using Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;
using Elaaj.Application.Features.Pharmacies.Commands.UpdatePharmacy;
using Elaaj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Pharmacies.Dtos
{
    public class PharmacyProfile : Profile
    {
        public PharmacyProfile()
        {
            CreateMap<Pharmacy, PharmacyDto>();
            CreateMap<CreatePharmacyCommand, Pharmacy>();
            CreateMap<UpdatePharmacyCommand, Pharmacy>();
        }
    }
}
