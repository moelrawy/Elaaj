using Elaaj.Application.Features.Users.UserDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<AuthResult>
    {
        public string Email { get; set; } = string.Empty;
    }
}
