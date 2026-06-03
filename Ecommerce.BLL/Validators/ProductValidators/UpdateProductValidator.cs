using FluentValidation;

namespace ECommerce.BLL{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .WithErrorCode("ERR_PROD_UPDATE_01")
                .MaximumLength(200)
                .WithMessage("Name cannot exceed 200 characters.")
                .WithErrorCode("ERR_PROD_UPDATE_02");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Description is required.")
                .WithErrorCode("ERR_PROD_UPDATE_03")
                .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters.")
                .WithErrorCode("ERR_PROD_UPDATE_04");

            RuleFor(x => x.Price)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.")
                .WithErrorCode("ERR_PROD_UPDATE_05");

            RuleFor(x => x.Stock)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative.")
                .WithErrorCode("ERR_PROD_UPDATE_06");

            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("A valid category is required.")
                .WithErrorCode("ERR_PROD_UPDATE_07");
        }
    }
}