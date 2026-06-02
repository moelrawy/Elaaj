using MediatR;
using Microsoft.AspNetCore.Http;

namespace Elaaj.Application.Features.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommand : IRequest
{
    public string? FullName { get; set; } 
    public DateOnly? DateOfBirth { get; set; }
    public IFormFile? ImageFile { get; set; } 
    public string? Address { get; set; }
    public double? Latitude { get; set; } 
    public double? Longitude { get; set; }
}
