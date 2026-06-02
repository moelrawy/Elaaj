using System;
using System.Collections.Generic;

namespace Elaaj.Application.Features.Users.UserDtos
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}
