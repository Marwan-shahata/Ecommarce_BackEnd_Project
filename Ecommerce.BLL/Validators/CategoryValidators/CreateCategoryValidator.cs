using FluentValidation;

namespace ECommerce.BLL
{

    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .WithErrorCode("ERR_CAT_CREATE_01")
                .MinimumLength(3)
                .WithMessage("Category name must be at least 3 characters.")
                .WithErrorCode("ERR_CAT_CREATE_02")
                .MaximumLength(100)
                .WithMessage("Category name must not exceed 100 characters.")
                .WithErrorCode("ERR_CAT_CREATE_03");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.")
                .WithErrorCode("ERR_CAT_CREATE_04")
                .When(x => x.Description != null);
        }
    }
}