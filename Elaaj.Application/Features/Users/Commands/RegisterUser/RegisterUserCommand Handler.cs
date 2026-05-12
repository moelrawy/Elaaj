using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Elaaj.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResult?>
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserCommandHandler(
        IAuthService authService,
        IValidator<RegisterUserCommand> validator)
    {
        _authService = authService;
        _validator = validator;
    }
    public async Task<AuthResult?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return new AuthResult
            {
                Success = false,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
        return await _authService.RegisterAsync(request.FullName, request.Email, request.Password);
    }
}