using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI.Controllers
{
    [Route("api")]
    public class StartPointController : ControllerBase
    {
        [HttpGet]
        public ActionResult Start()
        {
            return Ok();
        }
    }
}