using FluentValidation;
using RestaurantAPI.Entities;
using System.Linq;

namespace RestaurantAPI.Models.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(c => c.Password).NotEmpty();
            RuleFor(c => c.Password).MinimumLength(8);

            RuleFor(c => c.ConfirmPassword).Custom((value, context) =>
            {
                if (!value.Equals(context.InstanceToValidate.Password))
                {
                    context.AddFailure("ConfirmPassword", "The passwords are different");
                }
            });

            RuleFor(c => c.Email).NotEmpty();
            RuleFor(c => c.Email).EmailAddress();
            RuleFor(c => c.Email).Custom((value, context) =>
            {
                var emailAlreadyUsed = dbContext.Users.Any(u => u.Email == value);
                if (emailAlreadyUsed)
                {
                    context.AddFailure("Email", "Email already used by another account");
                }
            });
        }
    }
}