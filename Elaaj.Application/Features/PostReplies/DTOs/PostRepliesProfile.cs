using AutoMapper;
using Elaaj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.PostReplies.DTOs;

public class PostRepliesProfile : Profile
{
    public PostRepliesProfile()
    {
        CreateMap<PostReply, PostReplyDto>()
            .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.Pharmacy.Name));
    }
}
