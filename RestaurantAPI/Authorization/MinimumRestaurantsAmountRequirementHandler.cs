using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RestaurantAPI.Entities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization
{
    public class MinimumRestaurantsAmountRequirementHandler : AuthorizationHandler<MinimumRestaurantsAmountRequirement>
    {
        private readonly RestaurantDbContext _dbContext;

        public MinimumRestaurantsAmountRequirementHandler(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumRestaurantsAmountRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var userRestaurantsAmount = _dbContext.Restaurants.Count(r => r.CreatedById == userId);
            if (userRestaurantsAmount >= requirement.MinRestaurantAmount)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}