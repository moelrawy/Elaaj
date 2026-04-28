using MediatR;

namespace Elaaj.Application.Features.Users.Commands.UnAssignUserRole;

public class UnAssignUserRoleCommand : IRequest
{
    public string UserEmail { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}
