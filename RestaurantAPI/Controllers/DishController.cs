using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : Controller
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost]
        public ActionResult CreateDish([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            int dishId = _dishService.Create(restaurantId, dto);
            return Created($"api/{restaurantId}/dish/{dishId}", null);
        }
    }
}