using FluentValidation;

namespace RestaurantAPI.Models.Validators
{
    public class CreateRestaurantDtoValidator : AbstractValidator<CreateRestaurantDto>
    {
        public CreateRestaurantDtoValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Name).MaximumLength(25);
            RuleFor(c => c.ContactEmail).NotEmpty();
            RuleFor(c => c.ContactEmail).EmailAddress();
            RuleFor(c => c.ContactNumber).NotEmpty();
            RuleFor(c => c.City).NotEmpty();
            RuleFor(c => c.Street).NotEmpty();
            RuleFor(c => c.City).MaximumLength(50);
            RuleFor(c => c.Street).MaximumLength(50);
        }
    }
}