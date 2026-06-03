using FluentValidation;

namespace ECommerce.BLL.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .WithErrorCode("ERR_AUTH_LOGIN_01")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .WithErrorCode("ERR_AUTH_LOGIN_02");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required.")
            .WithErrorCode("ERR_AUTH_LOGIN_03");
    }
}