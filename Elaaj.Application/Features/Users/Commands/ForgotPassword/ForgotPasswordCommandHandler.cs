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

            // Generate OTP (6 digits)
            var otp = GenerateOtp();

            // Save OTP to store (expires in 10 minutes)
            OtpStore.SaveOtp(user.Id, otp, expirationMinutes: 10);

            // Send OTP via email
            var subject = "استرجاع كلمة المرور - Elaaj";
            var body = $@"
            <html dir='rtl'>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; direction: rtl; }}
                    .container {{ max-width: 500px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 8px; }}
                    .header {{ color: #2c3e50; text-align: center; margin-bottom: 20px; }}
                    .otp-box {{ background-color: #e74c3c; color: white; padding: 15px; border-radius: 5px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 3px; margin: 20px 0; }}
                    .footer {{ text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2 class='header'>استرجاع كلمة المرور</h2>
                    <p>تم طلب استرجاع كلمة المرور لحسابك في Elaaj</p>
                    <p>استخدم الكود التالي لإعادة تعيين كلمة مرورك:</p>
                    <div class='otp-box'>{otp}</div>
                    <p>هذا الكود صالح لمدة 10 دقائق فقط.</p>
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
                OtpStore.RemoveOtp(user.Id);
                return new AuthResult
                {
                    Success = false,
                    Message = "فشل إرسال رسالة استرجاع كلمة المرور"
                };
            }

            // Log for testing
            System.Diagnostics.Debug.WriteLine($"Forgot Password OTP for {user.Email}: {otp}");

            return new AuthResult
            {
                Success = true,
                Message = "تم إرسال رمز التحقق إلى بريدك الإلكتروني",
               // Data = new { userId = user.Id }
            };
        }

        // Generate random 6-digit OTP
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
