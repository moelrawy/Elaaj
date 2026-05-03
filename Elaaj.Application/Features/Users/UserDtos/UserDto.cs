namespace Elaaj.Application.Features.UserDtos;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? imageUrl { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}