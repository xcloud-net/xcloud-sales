using FluentValidation;
using XCloud.Core.Dto;

namespace XCloud.Platform.Application.Member.Validation;

public class UserPasswordLoginDtoValidator : AbstractValidator<PasswordLoginDto>
{
    public UserPasswordLoginDtoValidator()
    {
        RuleFor(x => x.IdentityName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(20).WithMessage("用户名最大长度为20");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(20).WithMessage("密码最大长度20");
    }
}