using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Elaaj.Application.Features.Users.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, AuthResult>
{
    private readonly IAuthService _authService;
    private readonly IValidator<VerifyEmailCommand> _validator;

    public VerifyEmailCommandHandler(
        IAuthService authService,
        IValidator<VerifyEmailCommand> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<AuthResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
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

        return await _authService.VerifyEmailAsync(request.Email, request.Code);
    }
}