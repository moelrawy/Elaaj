namespace Elaaj.Application.Features.Users;

public record CurrentUser(
    string Id,
    string Email,
    IEnumerable<string> Roles,
    DateOnly? DateOfBirth)
{
    public bool IsInRole(string role) => Roles.Contains(role);
}
