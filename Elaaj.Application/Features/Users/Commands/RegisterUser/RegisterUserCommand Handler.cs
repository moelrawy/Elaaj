using Elaaj.Application.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(IAuthService authService)
    : IRequestHandler<RegisterUserCommand, string?>
{
    public async Task<string?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // بينادي الـ Service اللي إحنا جهزناها قبل كدة
        return await authService.RegisterAsync(request.FullName, request.Email, request.Password);
    }
}