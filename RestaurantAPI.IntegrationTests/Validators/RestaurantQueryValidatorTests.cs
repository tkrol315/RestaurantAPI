using FluentValidation.TestHelper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTests.Validators
{
    public class RestaurantQueryValidatorTests
    {
        private static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RestaurantQuery>()
                {
                    new RestaurantQuery()
                    {
                         PageNumber = 1,
                         PageSize = 10,
                    },
                    new RestaurantQuery()
                    {
                         PageNumber = 2,
                         PageSize = 15,
                    },
                     new RestaurantQuery()
                    {
                         PageNumber = 10,
                         PageSize = 5,
                    },
                      new RestaurantQuery()
                    {
                         PageNumber = 1,
                         PageSize = 10,
                         SortBy = nameof(Restaurant.Name)
                    },
                        new RestaurantQuery()
                    {
                         PageNumber = 2,
                         PageSize = 15,
                         SortBy = nameof(Restaurant.Category)
                    }
                };
            return list.Select(q => new object[] { q });
        }

        private static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RestaurantQuery>()
                {
                    new RestaurantQuery()
                    {
                         PageNumber = 0,
                         PageSize = 10,
                    },
                    new RestaurantQuery()
                    {
                         PageNumber = 2,
                         PageSize = 9,
                    },
                     new RestaurantQuery()
                    {
                         PageNumber = -1,
                         PageSize = 5,
                    },
                      new RestaurantQuery()
                    {
                         PageNumber = -1,
                         PageSize = -1,
                    },
                        new RestaurantQuery()
                    {
                         PageNumber = 2,
                         PageSize = 15,
                         SortBy = nameof(Restaurant.ContactNumber)
                    }
                };
            return list.Select(q => new object[] { q });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnsSuccess(RestaurantQuery model)
        {
            var validator = new RestaurantQueryValidator();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(RestaurantQuery model)
        {
            var validator = new RestaurantQueryValidator();

            var result = validator.TestValidate(model);

            result.ShouldHaveAnyValidationError();
        }
    }
}
