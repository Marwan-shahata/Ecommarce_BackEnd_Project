using FluentValidation;

namespace ECommerce.BLL
{

    public class PlaceOrderValidator : AbstractValidator<PlaceOrderDto>
    {
        public PlaceOrderValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Shipping address is required.")
                .WithErrorCode("ERR_ORDER_CREATE_01")
                .MaximumLength(500);

            RuleFor(x => x.City)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("City is required.")
                .WithErrorCode("ERR_ORDER_CREATE_02")
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Postal code is required.")
                .WithErrorCode("ERR_ORDER_CREATE_03")
                .MaximumLength(20);

            RuleFor(x => x.Country)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Country is required.")
                .WithErrorCode("ERR_ORDER_CREATE_04")
                .MaximumLength(100);

            RuleFor(x => x.Notes)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.")
                .WithErrorCode("ERR_ORDER_CREATE_05")
                .When(x => x.Notes != null);
        }
    }
}