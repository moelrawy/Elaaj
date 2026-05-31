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

namespace Elaaj.Application.Features.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, AuthResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<AuthResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "البريد الإلكتروني غير مسجل"
                };
            }

            // Check if email is not null
            if (string.IsNullOrEmpty(user.Email))
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "خطأ في بيانات البريد الإلكتروني"
                };
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send email with reset link
            var resetLink = $"https://yourapp.com/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            // ✅ أضيف السطر ده للـ testing
            System.Diagnostics.Debug.WriteLine($"Reset Link: {resetLink}");

            var subject = "استرجاع كلمة المرور - Elaaj";
            var body = $@"
            <html dir='rtl'>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; direction: rtl; }}
                    .container {{ max-width: 500px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 8px; }}
                    .header {{ color: #2c3e50; text-align: center; margin-bottom: 20px; }}
                    .button {{ background-color: #e74c3c; color: white; padding: 12px 30px; border-radius: 5px; text-decoration: none; display: inline-block; margin: 20px 0; text-align: center; }}
                    .footer {{ text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2 class='header'>استرجاع كلمة المرور</h2>
                    <p>تم طلب استرجاع كلمة المرور لحسابك في Elaaj</p>
                    <p>اضغط على الزرار أدناه لإعادة تعيين كلمة مرورك:</p>
                    <div style='text-align: center;'>
                        <a href='{resetLink}' class='button'>إعادة تعيين كلمة المرور</a>
                    </div>
                    <p>أو انسخ الرابط التالي في المتصفح:</p>
                    <p style='word-break: break-all; color: #3498db;'>{resetLink}</p>
                    <p>هذا الرابط صالح لمدة 24 ساعة فقط.</p>
                    <p style='color: #e74c3c; font-weight: bold;'>إذا لم تقم بطلب هذا، يرجى تجاهل هذا البريد وتغيير كلمة مرورك فوراً.</p>
                    <div class='footer'>
                        <p>© 2025 Elaaj Pharmacy. جميع الحقوق محفوظة.</p>
                    </div>
                </div>
            </body>
            </html>
        ";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "فشل إرسال رسالة استرجاع كلمة المرور"
                };
            }

            return new AuthResult
            {
                Success = true,
                Message = "تم إرسال رسالة استرجاع كلمة المرور إلى بريدك الإلكتروني",
            };
        }
    }
}
