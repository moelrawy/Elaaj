using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Elaaj.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    IUserContext userContext,
    UserManager<User> userManager) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userContextData = userContext.GetCurrentUser();
        if (userContextData == null) throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً.");

        var user = await userManager.FindByIdAsync(userContextData.Id);
        if (user == null) throw new Exception("المستخدم غير موجود.");

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new Exception("فشل في حذف حساب المستخدم.");
    }
}
