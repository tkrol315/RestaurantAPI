using FluentValidation;

namespace RestaurantAPI.Models.Validators
{
    public class UpdateRestaurantDtoValidator : AbstractValidator<UpdateRestaurantDto>
    {
        public UpdateRestaurantDtoValidator()
        {
            RuleFor(u => u.Name).NotEmpty();
            RuleFor(u => u.Name).MaximumLength(25);
        }
    }
}