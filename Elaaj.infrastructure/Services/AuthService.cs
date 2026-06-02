using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Application.Interfaces;
using Elaaj.Application.Interfaces.Services;
using Elaaj.Application.Models;
using Elaaj.Domain.Constants;
using Elaaj.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Elaaj.infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService; // Added Email Service

    public AuthService(UserManager<User> userManager, IOptions<JwtSettings> jwtSettings, IEmailService emailService)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _emailService = emailService;
    }

    public async Task<AuthResult?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return new AuthResult { Success = false, Errors = new List<string> { "بيانات الدخول غير صحيحة" } };
            
        // Prevent login if email is not confirmed
        if (!user.EmailConfirmed)
            return new AuthResult { Success = false, Errors = new List<string> { "يرجى تأكيد بريدك الإلكتروني أولاً" } };

        // Generate tokens
        var token = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // الـ Refresh Token يستمر لمدة 7 أيام مثلاً
        await _userManager.UpdateAsync(user);

        return new AuthResult { Success = true, Token = token, RefreshToken = refreshToken };
    }

    public async Task<AuthResult?> RegisterAsync(string fullName, string email, string password)
    {
        var otpCode = new Random().Next(100000, 999999).ToString();
        
        User user = new User
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = false,
            EmailConfirmationCode = otpCode,
            EmailConfirmationCodeExpires = DateTime.UtcNow.AddMinutes(10)
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            // Send confirmation email
            var emailBody = $"<p>مرحباً {fullName},</p><p>رمز التأكيد الخاص بك هو: <strong>{otpCode}</strong></p><p>هذا الرمز صالح لمدة 10 دقائق.</p>";
            await _emailService.SendEmailAsync(user.Email, "تأكيد حسابك", emailBody);

            return new AuthResult { Success = true };
        }

        return new AuthResult 
        { 
            Success = false, 
            Errors = result.Errors.Select(e => e.Description).ToList() 
        };
    }
    
    // Ensure this method is implemented
    public async Task<AuthResult> VerifyEmailAsync(string email, string code)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new AuthResult { Success = false, Errors = new List<string> { "المستخدم غير موجود" } };

        if (user.EmailConfirmed)
            return new AuthResult { Success = false, Errors = new List<string> { "الحساب مفعل بالفعل" } };

        if (user.EmailConfirmationCode != code || user.EmailConfirmationCodeExpires < DateTime.UtcNow)
            return new AuthResult { Success = false, Errors = new List<string> { "رمز التأكيد غير صحيح أو منتهي الصلاحية" } };

        // Mark as confirmed and clean up the code
        user.EmailConfirmed = true;
        user.EmailConfirmationCode = null;
        user.EmailConfirmationCodeExpires = null;

        await _userManager.UpdateAsync(user);

        return new AuthResult { Success = true };
    }

    public async Task<AuthResult?> RefreshTokenAsync(string refreshToken)
    {
        var users = _userManager.Users.Where(u => u.RefreshToken == refreshToken).ToList();
        var user = users.FirstOrDefault();

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResult { Success = false, Message = "الـ Refresh Token غير صالح أو انتهت صلاحيته" };
        }

        var newAccessToken = await GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthResult 
        { 
            Success = true, 
            Token = newAccessToken, 
            RefreshToken = newRefreshToken 
        };
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}