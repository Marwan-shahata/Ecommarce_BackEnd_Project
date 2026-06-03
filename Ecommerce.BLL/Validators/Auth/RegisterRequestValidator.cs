using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
   public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("First name is required.")
            .WithErrorCode("ERR_AUTH_REGISTER_01")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters.")
            .WithErrorCode("ERR_AUTH_REGISTER_02");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .WithErrorCode("ERR_AUTH_REGISTER_03")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters.")
            .WithErrorCode("ERR_AUTH_REGISTER_04");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email is required.")
            .WithErrorCode("ERR_AUTH_REGISTER_05")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .WithErrorCode("ERR_AUTH_REGISTER_06");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required.")
            .WithErrorCode("ERR_AUTH_REGISTER_07")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .WithErrorCode("ERR_AUTH_REGISTER_08")
            .Matches("[A-Z]")
            .WithMessage("Password must contain uppercase letter.")
            .WithErrorCode("ERR_AUTH_REGISTER_09")
            .Matches("[a-z]")
            .WithMessage("Password must contain lowercase letter.")
            .WithErrorCode("ERR_AUTH_REGISTER_10")
            .Matches("[0-9]")
            .WithMessage("Password must contain a digit.")
            .WithErrorCode("ERR_AUTH_REGISTER_11")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain special character.")
            .WithErrorCode("ERR_AUTH_REGISTER_12");

        RuleFor(x => x.ConfirmPassword)
            .Cascade(CascadeMode.Stop)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.")
            .WithErrorCode("ERR_AUTH_REGISTER_13");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Invalid phone number format.")
            .WithErrorCode("ERR_AUTH_REGISTER_14")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
    }
}