using Elaaj.Application.DTOs;
using Elaaj.Application.Features.Users.Commands.RegisterUser;
using Elaaj.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;


    public AuthController(IAuthService authService, IMediator mediator)
    {
        _authService = authService;
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

        if (token == null)
            return Unauthorized(new { Message = "بيانات الدخول غير صحيحة" });

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var authResult = await _mediator.Send(command);

        if (!authResult.Success)
        {
            return BadRequest(new { Errors = authResult.Errors, Message = "فشل إنشاء الحساب" });
        }

        return Ok(new { Token = authResult.Token, Message = "تم إنشاء الحساب بنجاح" });
    }
}