using MediatR;

namespace Elaaj.Application.Features.PharmacyAdmins.Commands.DeleteAdmin;

public class DeletePharmacyAdminCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
}