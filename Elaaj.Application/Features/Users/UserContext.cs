using Elaaj.Application.Features.Users;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Globalization;

namespace Elaaj.Application.Users
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }

    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                throw new InvalidOperationException("User context is not present");
            }

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }
            
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? throw new InvalidOperationException("User ID claim is missing");
            var email = user.FindFirst(ClaimTypes.Email)?.Value
                        ?? throw new InvalidOperationException("Email claim is missing");
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            var dateOfBirthString = user.FindFirst("DateOfBirth")?.Value;
            var dateOfBirth = dateOfBirthString == null
                ? (DateOnly?)null
                : DateOnly.ParseExact(dateOfBirthString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var profileImageUrl = user.FindFirst("ProfileImageUrl")?.Value;

            var latClaim = user.FindFirst("Latitude")?.Value;
            var lngClaim = user.FindFirst("Longitude")?.Value;

            double? latitude = double.TryParse(latClaim, out var lat) ? lat : null;
            double? longitude = double.TryParse(lngClaim, out var lng) ? lng : null;

            return new CurrentUser(
                userId,
                email,
                roles,
                dateOfBirth,
                profileImageUrl,
                latitude,
                longitude);
        }
    }
}