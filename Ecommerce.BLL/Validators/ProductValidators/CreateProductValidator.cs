using FluentValidation;

namespace ECommerce.BLL
{

    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .WithErrorCode("ERR_PROD_CREATE_01")
                .MaximumLength(200)
                .WithMessage("Name cannot exceed 200 characters.")
                .WithErrorCode("ERR_PROD_CREATE_02");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Description is required.")
                .WithErrorCode("ERR_PROD_CREATE_03")
                .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters.")
                .WithErrorCode("ERR_PROD_CREATE_04");

            RuleFor(x => x.Price)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.")
                .WithErrorCode("ERR_PROD_CREATE_05");

            RuleFor(x => x.Stock)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative.")
                .WithErrorCode("ERR_PROD_CREATE_06");

            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("A valid category is required.")
                .WithErrorCode("ERR_PROD_CREATE_07");
        }
    }
}