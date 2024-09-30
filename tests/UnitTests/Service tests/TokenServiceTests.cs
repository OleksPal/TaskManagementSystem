using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.UnitTests
{
    [Collection("TestCollection")]
    public class TokenServiceTests
    {
        protected readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _tokenService = Helper.MockTokenService().Object;
        }

        #region CreateToken
        [Fact]
        public void CreateToken_Null_ReturnsNullReferenceException()
        {
            // Arrange
            User user = null;

            // Act
            Action act = () => _tokenService.CreateToken(user);

            // Assert
            Assert.Throws<NullReferenceException>(act);
        }

        [Fact]
        public void CreateToken_InvalidUserWithoutRequiredProperties_ReturnsArgumentNullException()
        {
            // Arrange
            var user = new User();

            // Act
            Action act = () => _tokenService.CreateToken(user);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void CreateToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser",
                Email = "test@gmail.com",
                PasswordHash = String.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            Assert.NotNull(token);
        }
        #endregion
    }
}
