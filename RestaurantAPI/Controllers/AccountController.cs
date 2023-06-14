using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] CreateUserDto dto)
        {
            _accountService.CreateAccount(dto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginUserDto dto)
        {
            var token = _accountService.GenerateJwt(dto);
            return Ok(token);
        }
    }
}