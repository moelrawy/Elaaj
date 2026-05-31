using Elaaj.Application.DTOs;
using Elaaj.Application.Features.Users.Commands.ForgotPassword;
using Elaaj.Application.Features.Users.Commands.RegisterUser;
using Elaaj.Application.Features.Users.Commands.ResetPassword;
using Elaaj.Application.Features.Users.Commands.VerifyEmail;
using Elaaj.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Elaaj.API.Controllers;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var authResult = await _mediator.Send(command);

        if (!authResult.Success)
        {
            return BadRequest(new { Errors = authResult.Errors, Message = "فشل إنشاء الحساب" });
        }

        // We don't return the token here anymore because the user must verify their email first
        return Ok(new { Message = "تم إنشاء الحساب بنجاح. يرجى مراجعة بريدك الإلكتروني لتفعيل الحساب." });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        var authResult = await _mediator.Send(command);

        if (!authResult.Success)
        {
            return BadRequest(new { Errors = authResult.Errors, Message = "فشل تفعيل الحساب" });
        }

        return Ok(new { Message = "تم تفعيل الحساب بنجاح. يمكنك الآن تسجيل الدخول." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var authResult = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

        if (authResult == null || !authResult.Success)
        {
            return Unauthorized(new
            {
                Errors = authResult?.Errors,
                Message = "بيانات الدخول غير صحيحة أو الحساب غير مفعل"
            });
        }

        return Ok(new { Token = authResult.Token });
    }

    

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { Message = result.Message, Errors = result.Errors });
        }

        return Ok(new { Message = result.Message, Data = result.Data });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { Message = result.Message, Errors = result.Errors });
        }

        return Ok(new { Message = result.Message });
    }

   
}