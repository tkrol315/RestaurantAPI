using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTests.Validators
{
    public class RestaurantUserValidatorTests
    {
        private readonly RestaurantDbContext _dbContext;
        public RestaurantUserValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
            builder.UseInMemoryDatabase("RestaurantDb");

            _dbContext = new RestaurantDbContext(builder.Options);

            Seed();
        }

        private void Seed()
        {
            var testUsers = new List<User>() 
            { 
                new User()
                {
                    Email = "testEmail@TestEmail.com"
                },
                  new User()
                {
                    Email = "testEmail2@TestEmail2.com"
                }
            };

            _dbContext.Users.AddRange(testUsers);
            _dbContext.SaveChanges();
        }

        private static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<CreateUserDto>()
            {
              new CreateUserDto()
              {
                Email = "Test@Test.com",
                Password = "password123",
                ConfirmPassword = "password123"
              },
              new CreateUserDto()
              {
                Email = "Test123@Test.com",
                Password = "passpass!@#$",
                ConfirmPassword = "passpass!@#$"
              },
               new CreateUserDto()
              {
                Email = "Valid@Test.com",
                Password = "ValidPassword123",
                ConfirmPassword = "ValidPassword123"
              },

            };
            return list.Select(c => new object[]{ c });
        }

        private static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<CreateUserDto>()
            {
                new CreateUserDto()
                {
                    Email = "testEmail@TestEmail.com",
                    Password = "Password123",
                    ConfirmPassword = "Password123"
                },
                new CreateUserDto()
                {
                  Email = "testEmail2@TestEmail2.com",
                  Password = "Password123",
                  ConfirmPassword = "Password123"
                },
                new CreateUserDto()
                {
                    Email = "ValidEmailAddress@Test.com",
                    Password = "Pass",
                    ConfirmPassword = "Pass"
                },
            };
            return list.Select(c => new object[] { c });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForValidModel_ReturnsSuccess(CreateUserDto dto)
        {  
            var validator = new CreateUserDtoValidator(_dbContext);
            
            var result = validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForInvalidModel_ReturnsFailure(CreateUserDto dto)
        {
            var validator = new CreateUserDtoValidator(_dbContext);

            var result = validator.TestValidate(dto);

            result.ShouldHaveAnyValidationError();
        }
    }
}
