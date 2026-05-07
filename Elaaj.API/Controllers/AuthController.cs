using Elaaj.Application.DTOs;
using Elaaj.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

        if (token == null)
            return Unauthorized(new { Message = "بيانات الدخول غير صحيحة" });

        return Ok(new { Token = token });
    }
}