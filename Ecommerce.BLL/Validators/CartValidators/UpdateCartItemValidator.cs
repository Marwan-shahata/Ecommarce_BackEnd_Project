using FluentValidation;

namespace ECommerce.BLL
{
    public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemDto>
    {
        public UpdateCartItemValidator()
        {
            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Valid product ID is required.")
                .WithErrorCode("ERR_CART_UPDATE_01");

            RuleFor(x => x.Quantity)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity cannot be negative.")
                .WithErrorCode("ERR_CART_UPDATE_02");
        }
    }

    
}