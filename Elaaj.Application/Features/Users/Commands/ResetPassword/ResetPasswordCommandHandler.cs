using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Application.Interfaces.Services;
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
            // Validate passwords match
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "كلمات المرور غير متطابقة"
                };
            }

            // Find user
            //var userId = OtpStore.GetUserIdByOtp(request.Otp);
            //if (userId == null)
            //{
            //    return new AuthResult
            //    {
            //        Success = false,
            //        Message = "المستخدم غير موجود"
            //    };
            //}


            // Verify OTP
            var userId = OtpStore.GetUserIdByOtp(request.Otp);
            if (userId == null)
            {
                return new AuthResult { Success = false, Message = "رمز التحقق غير صحيح أو انتهت صلاحيته" };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResult { Success = false, Message = "المستخدم غير موجود" };
            }

            // Remove password hash (reset password)
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "فشل تحديث كلمة المرور",
                    Errors = removePasswordResult.Errors.Select(e => e.Description).ToList()
                };
            }

            // Add new password
            var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "فشل تحديث كلمة المرور",
                    Errors = addPasswordResult.Errors.Select(e => e.Description).ToList()
                };
            }

            // Remove OTP from store after successful verification
            OtpStore.RemoveOtp(request.Otp);

            return new AuthResult
            {
                Success = true,
                Message = "تم تحديث كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول بكلمة المرور الجديدة"
            };
        }
    }
}
