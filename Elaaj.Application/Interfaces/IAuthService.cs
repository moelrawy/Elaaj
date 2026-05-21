using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces;

public interface IAuthService
{
<<<<<<< Updated upstream
    Task<string?> RegisterAsync(string fullName, string email, string password);
    Task<string?> LoginAsync(string email, string password);
=======
    Task<AuthResult?> RegisterAsync(string fullName, string email, string password);
    Task<AuthResult?> LoginAsync(string email, string password); // ✅ غيرنا من string? لـ AuthResult?
>>>>>>> Stashed changes
}
