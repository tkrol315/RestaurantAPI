using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        void CreateAccount(CreateUserDto dto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _dbContext;

        public AccountService(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateAccount(CreateUserDto dto)
        {
            var user = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId,
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
    }
}