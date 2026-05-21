using Elaaj.Application.Features.Users.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult?> RegisterAsync(string fullName, string email, string password);
    Task<AuthResult?> LoginAsync(string email, string password);
}
