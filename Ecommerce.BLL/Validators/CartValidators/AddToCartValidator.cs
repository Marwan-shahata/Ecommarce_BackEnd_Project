using FluentValidation;

namespace ECommerce.BLL
{
    public class AddToCartValidator : AbstractValidator<AddToCartDto>
    {
        public AddToCartValidator()
        {
            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Valid product ID is required.")
                .WithErrorCode("ERR_CART_ADD_01");

            RuleFor(x => x.Quantity)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Quantity must be at least 1.")
                .WithErrorCode("ERR_CART_ADD_02")
                .LessThanOrEqualTo(100)
                .WithMessage("Cannot add more than 100 items.")
                .WithErrorCode("ERR_CART_ADD_03");
        }
    }
}