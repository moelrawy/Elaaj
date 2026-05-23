using Elaaj.Application.Features.Users.Commands.VerifyEmail;
using FluentValidation;

namespace Elaaj.Application.Features.Users.UserValidations;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("«·»—Ìœ «·≈·ﬂ —Ê‰Ì „ÿ·Ê».")
            .EmailAddress().WithMessage("’Ì€… «·»—Ìœ «·≈·ﬂ —Ê‰Ì €Ì— ’ÕÌÕ….");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("—„“ «· √ﬂÌœ „ÿ·Ê».")
            .Length(6).WithMessage("ÌÃ» √‰ Ì ﬂÊ‰ —„“ «· √ﬂÌœ „‰ 6 √—ﬁ«„.");
    }
}