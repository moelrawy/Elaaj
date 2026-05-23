using Elaaj.Application.Features.Users.UserDtos;
using MediatR;

namespace Elaaj.Application.Features.Users.Commands.VerifyEmail;

public class VerifyEmailCommand : IRequest<AuthResult>
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}