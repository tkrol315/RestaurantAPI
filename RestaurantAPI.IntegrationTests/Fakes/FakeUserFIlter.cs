﻿using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace RestaurantAPI.IntegrationTests
{
    public class FakeUserFIlter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "1")
                }));
            context.HttpContext.User = claimsPrincipal;

            await next();
        }
    }
}
