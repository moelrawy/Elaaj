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
        var authResult = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

        if (authResult == null || !authResult.Success)
            return Unauthorized(new { Message = "بيانات الدخول غير صحيحة" });

        return Ok(new { Token = authResult.Token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        // الـ FluentValidation هيشتغل لوحده هنا قبل ما السطر اللي تحت يتنفذ
        var token = await _mediator.Send(command);

        if (token == null)
        {
            return BadRequest(new { Message = "فشل التسجيل. تأكد من البيانات أو أن الإيميل غير مستخدم." });
        }

        return Ok(new { Token = token, Message = "تم إنشاء الحساب بنجاح" });
    }
}