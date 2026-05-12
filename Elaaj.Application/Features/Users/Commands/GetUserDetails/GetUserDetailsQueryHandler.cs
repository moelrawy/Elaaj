using Elaaj.Application.Features.Users.Commands.GetUserDetails;
using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Elaaj.Application.Features.Users.Queries.GetUserDetails;

public class GetUserDetailsQueryHandler(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetUserDetailsQuery, UserDetailsDto?>
{
    public async Task<UserDetailsDto?> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        // 1. نجيب الـ User ID من الـ Token اللي باعت الـ Request
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null) return null;

        // 2. ندور على اليوزر في الداتابيز
        var user = await userManager.FindByIdAsync(userId);

        if (user == null) return null;

        // 3. نحول بيانات اليوزر لـ DTO عشان نبعتها
        return new UserDetailsDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            ImageUrl = user.imageUrl,
            Address = user.Address,
            Latitude = user.Latitude,
            Longitude = user.Longitude,
            DateOfBirth = user.DateOfBirth
        };
    }
}