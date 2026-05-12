using AutoMapper;
using Elaaj.Application.Users;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Elaaj.Application.Features.Pharmacies.Commands.CreatePharmacy;

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, Guid>
{
    private readonly IGenericRepository<Pharmacy> _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext; 
    private readonly UserManager<User> _userManager;

    public CreatePharmacyCommandHandler(
        IGenericRepository<Pharmacy> repository,
        IMapper mapper,
        IUserContext userContext,
        UserManager<User> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        // 1. الحصول على اليوزر الحالي من التوكن (الأمان بيبدأ من هنا)
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        // 2. تحويل الـ Request لـ Entity
        var pharmacy = _mapper.Map<Pharmacy>(request);

        // 3. الربط الأوتوماتيكي: هنا بنضمن إن الصيدلية ملك للي فتحها
        pharmacy.OwnerId = currentUser.Id;

        // 4. إضافة المالك لجدول الـ Admins الخاص بالصيدلية (لإدارة الصلاحيات الداخلية)
        pharmacy.Admins.Add(new PharmacyAdmin
        {
            UserId = currentUser.Id,
            // ملحوظة: الـ PharmacyId هيتحدد أوتوماتيك بواسطة EF Core عند الحفظ
            Role = UserRoles.PharmacyOwner
        });

        // 5. حفظ الصيدلية في الداتابيز
        await _repository.AddAsync(pharmacy);
        await _repository.SaveChangesAsync();

        // 6. ترقية اليوزر في نظام الـ Identity (لو مش واخد الرول)
        // ده بيضمن إنه يقدر يوصل لـ Endpoints التعديل بعد كدة
        var user = await _userManager.FindByIdAsync(currentUser.Id);
        if (user != null)
        {
            var isInRole = await _userManager.IsInRoleAsync(user, UserRoles.PharmacyOwner);
            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.PharmacyOwner);
                // ملحوظة: اليوزر هيحتاج يعمل Login جديد عشان الرول تظهر في التوكن بتاعه
            }
        }

        return pharmacy.Id;
    }
}
