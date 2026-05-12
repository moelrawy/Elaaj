using Elaaj.Application.Features.Users.Commands.RegisterUser;
using FluentValidation;

namespace Elaaj.Application.Features.Identity;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم بالكامل مطلوب")
            .MinimumLength(3).WithMessage("الاسم يجب ألا يقل عن 3 حروف");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة المرور مطلوبة")
            .MinimumLength(6).WithMessage("كلمة المرور يجب ألا تقل عن 6 أحرف");

        // هنا بنعمل الـ Confirm Password اللي كنت بتسأل عليه
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("كلمة المرور غير متطابقة");
    }
}