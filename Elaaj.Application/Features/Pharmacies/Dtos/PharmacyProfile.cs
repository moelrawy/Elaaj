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
            CreateMap<Pharmacy, PharmacyDto>()
            .ForMember(dest => dest.Distance, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<CreatePharmacyCommand, Pharmacy>()
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Admins, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdatePharmacyCommand, Pharmacy>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
