using Elaaj.Application.Features.UserDtos;
using FluentValidation;

namespace Elaaj.Application.Features.Users.UserValidations
{
    public class UserValidation : AbstractValidator<UserDto>
    {
        public UserValidation()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("الاسم الكامل مطلوب")
                .MinimumLength(3).WithMessage("الاسم يجب ألا يقل عن 3 أحرف")
                .MaximumLength(100).WithMessage("الاسم طويل جداً");

            RuleFor(x => x.imageUrl)
                .MaximumLength(500).WithMessage("رابط الصورة طويل جداً");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("خط العرض (Latitude) غير صحيح")
                .When(x => x.Latitude.HasValue);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("خط الطول (Longitude) غير صحيح")
                .When(x => x.Longitude.HasValue);

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("العنوان طويل جداً");
        }
    }
}