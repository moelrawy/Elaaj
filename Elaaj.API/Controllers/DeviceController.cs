using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Application.DTOs;
using Elaaj.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace Elaaj.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;
        
        public DeviceController(IApplicationDbContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }
        [HttpPost("register-token")]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] RegisterTokenRequest request)
        {
            var currentUser=_userContext.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized();
            }
            var isTokenExists = await _context.UserDevices
                .AnyAsync(d => d.UserId == currentUser.Id && d.DeviceToken == request.DeviceToken);
            if (!isTokenExists)
            {
                var device = new UserDevices(currentUser.Id, request.DeviceToken);
                _context.UserDevices.Add(device);
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Token registered successfully" });
        }
    }
    public record RegisterTokenRequest(string DeviceToken);
}
