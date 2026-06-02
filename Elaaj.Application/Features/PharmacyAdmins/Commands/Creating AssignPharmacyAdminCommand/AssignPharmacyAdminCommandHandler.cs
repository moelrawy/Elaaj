using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Elaaj.Application.Features.PharmacyAdmins.Commands.AssignAdmin;

public class AssignPharmacyAdminCommandHandler : IRequestHandler<AssignPharmacyAdminCommand, bool>
{
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly IGenericRepository<PharmacyAdmin> _adminRepository;
    private readonly UserManager<User> _userManager;
    private readonly IUserContext _userContext;

    public AssignPharmacyAdminCommandHandler(
        IGenericRepository<Pharmacy> pharmacyRepository,
        IGenericRepository<PharmacyAdmin> adminRepository,
        UserManager<User> userManager,
        IUserContext userContext)
    {
        _pharmacyRepository = pharmacyRepository;
        _adminRepository = adminRepository;
        _userManager = userManager;
        _userContext = userContext;
    }

    public async Task<bool> Handle(AssignPharmacyAdminCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user from Token
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        // 2. Verify pharmacy exists
        var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId);
        if (pharmacy == null)
            throw new KeyNotFoundException("الصيدلية غير موجودة");

        // 3. Only the pharmacy owner or platform admin can assign admins
        bool isPharmacyOwner = pharmacy.OwnerId == currentUser.Id;
        bool isPlatformAdmin = currentUser.IsInRole(UserRoles.Owner);
        if (!isPharmacyOwner && !isPlatformAdmin)
            throw new UnauthorizedAccessException("بس صاحب الصيدلية يقدر يضيف أدمن");

        // 4. Verify the user to be assigned exists
        var userToAssign = await _userManager.FindByIdAsync(request.UserId);
        if (userToAssign == null)
            throw new KeyNotFoundException("اليوزر غير موجود");

        // 5. Check if already an admin
        var existingAdmin = await _adminRepository.GetFirstOrDefaultAsync(a =>
            a.UserId == request.UserId && a.PharmacyId == request.PharmacyId);
        if (existingAdmin != null)
            throw new InvalidOperationException("اليوزر ده بالفعل أدمن في الصيدلية دي");

        // 6. Add as pharmacy admin
        var admin = new PharmacyAdmin
        {
            UserId = request.UserId,
            PharmacyId = request.PharmacyId,
            Role = UserRoles.PharmacyAdmin,
            AssignedAt = DateTime.UtcNow
        };

        await _adminRepository.AddAsync(admin);
        await _adminRepository.SaveChangesAsync();

        // 7. Add PharmacyAdmin role in Identity
        if (!await _userManager.IsInRoleAsync(userToAssign, UserRoles.PharmacyAdmin))
        {
            await _userManager.AddToRoleAsync(userToAssign, UserRoles.PharmacyAdmin);
        }

        return true;
    }
}