using Elaaj.Application.Features.Users.UserDtos;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult?> RegisterAsync(string fullName, string email, string password);
    Task<AuthResult?> LoginAsync(string email, string password);
    Task<AuthResult> VerifyEmailAsync(string email, string code);
    Task<AuthResult?> RefreshTokenAsync(string refreshToken); 
}
