using AutoMapper;
using Elaaj.Domain.Entities;

namespace Elaaj.Application.Features.PostReplies.DTOs;

public class PostRepliesProfile : Profile
{
    public PostRepliesProfile()
    {
        CreateMap<PostReply, PostReplyDto>()
            .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.Pharmacy != null ? src.Pharmacy.Name : string.Empty))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message)); // Add this mapped line if you retain 'Comment' on your DTO
    }
}
