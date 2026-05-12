using MediatR;

namespace Elaaj.Application.Features.PharmacyAdmins.Commands.AssignAdmin;

public class AssignPharmacyAdminCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty; // ID of the user to assign as admin
    public Guid PharmacyId { get; set; }
}