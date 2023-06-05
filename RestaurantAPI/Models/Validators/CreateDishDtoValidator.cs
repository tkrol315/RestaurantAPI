using FluentValidation;

namespace RestaurantAPI.Models.Validators
{
    public class CreateDishDtoValidator : AbstractValidator<CreateDishDto>
    {
        public CreateDishDtoValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Price).NotEmpty();
        }
    }
}