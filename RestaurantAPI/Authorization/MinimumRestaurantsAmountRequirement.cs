using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumRestaurantsAmountRequirement : IAuthorizationRequirement
    {
        public int MinRestaurantAmount { get; }

        public MinimumRestaurantsAmountRequirement(int minRestaurantAmount)
        {
            MinRestaurantAmount = minRestaurantAmount;
        }
    }
}