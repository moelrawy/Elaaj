using AutoMapper;
using Elaaj.Application.Features.UserDtos;
using Elaaj.Domain.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl));

        CreateMap<UserDto, User>()
            .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.imageUrl))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}