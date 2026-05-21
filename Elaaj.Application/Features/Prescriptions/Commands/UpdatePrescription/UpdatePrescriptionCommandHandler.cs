using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Enums;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Commands.UpdatePrescription;

public class UpdatePrescriptionCommandHandler : IRequestHandler<UpdatePrescriptionCommand, bool>
{
    private readonly IGenericRepository<Prescription> _repository;
    private readonly IUserContext _userContext;
    public UpdatePrescriptionCommandHandler(IGenericRepository<Prescription> repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public async Task<bool> Handle(UpdatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        Users.CurrentUser? currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

        var prescription = await _repository.GetByIdAsync(request.Id);

        if (prescription == null)
            throw new Exception("الروشتة غير موجودة.");

        if (prescription.UserId != currentUser.Id)
            throw new UnauthorizedAccessException("غير مصرح لك بتعديل هذه الروشتة.");

        if (prescription.Status != PrescriptionStatus.Pending)
            throw new Exception("لا يمكن تعديل الروشتة لأنه تم الرد عليها بالفعل.");

        // تحديث البيانات
        prescription.Notes = request.Notes;

        _repository.Update(prescription);
        await _repository.SaveChangesAsync();

        return true;
    }
}
