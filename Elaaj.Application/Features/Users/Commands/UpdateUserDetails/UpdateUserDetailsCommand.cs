using MediatR;

namespace Elaaj.Application.Features.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommand : IRequest
{
    public string? FullName { get; set; } 
    public DateOnly? DateOfBirth { get; set; }
    public string? imageUrl { get; set; } 
    public double? Latitude { get; set; } 
    public double? Longitude { get; set; }
}
