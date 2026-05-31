using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, AuthResult>
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Check if passwords match
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "كلمات المرور غير متطابقة"
                };
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "البريد الإلكتروني غير موجود"
                };
            }

            // Reset password using the token
            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "فشل تحديث كلمة المرور",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new AuthResult
            {
                Success = true,
                Message = "تم تحديث كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول بكلمة المرور الجديدة"
            };
        }
    }
}
