using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Application.Interfaces;
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
using System.Linq; // Added for .Select
using System.Threading.Tasks;

namespace Elaaj.infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(UserManager<User> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return new AuthResult { Success = false, Errors = new List<string> { "Invalid credentials" } };

        // Generate token with roles
        var token = await GenerateJwtToken(user);
        return new AuthResult { Success = true, Token = token };
    }

    public async Task<AuthResult?> RegisterAsync(string fullName, string email, string password)
    {
        User user = new User
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // 1. Automatically assign User role to every new user
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            // 2. Generate token with roles
            var token = await GenerateJwtToken(user);
            return new AuthResult { Success = true, Token = token };
        }

        return new AuthResult 
        { 
            Success = false, 
            Errors = result.Errors.Select(e => e.Description).ToList() 
        };
    }

    // ✅ Made async to fetch roles from Database
    private async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // ✅ Get user roles from Database and add them to Token
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
}