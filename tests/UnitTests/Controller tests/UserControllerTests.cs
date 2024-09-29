using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.UnitTests
{
    [Collection("TestCollection")]
    public class UserControllerTests
    {
        protected readonly UserController _userController;

        public UserControllerTests()
        {
            var userManagerMock = Helper.MockUserManager<User>([Helper.ExistingUser]);
            var tokenServiceMock = Helper.MockTokenService();
            var signInManagerMock = Helper.MockSignInManager(userManagerMock.Object);
            var loggerMock = new Mock<ILogger<UserController>>();

            _userController = new UserController(userManagerMock.Object, tokenServiceMock.Object,
                signInManagerMock.Object, loggerMock.Object);
        }

        #region Register
        [Fact]
        public async Task Register_Null_ReturnsStatusCode500()
        {
            // Arrange
            RegisterDto registerDto = null;
            var errorMessage = "Object reference not set to an instance of an object.";

            // Act
            var actionResult = await _userController.Register(registerDto);

            // Assert
            var serverError = actionResult as ObjectResult;
            Assert.Equal(errorMessage, serverError.Value);
        }

        [Fact]
        public async Task Register_InvalidUserWithoutRequiredProperties_ReturnsStatusCode500()
        {
            // Arrange
            var registerDto = new RegisterDto();
            var errorMessage = "Value cannot be null. (Parameter 'value')";

            // Act
            var actionResult = await _userController.Register(registerDto);

            // Assert
            var serverError = actionResult as ObjectResult;
            Assert.Equal(errorMessage, serverError.Value);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsNewUserDto()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "validregister@g.c",
                UserName = "ValidUserRegister",
                Password = "!1Qqwertyuiop"
            };

            // Act
            var actionResult = await _userController.Register(registerDto);

            // Assert
            var okResult = actionResult as ObjectResult;
            var newUserDto = okResult.Value as NewUserDto;
            Assert.NotNull(newUserDto);
        }
        #endregion

        #region LoginWithUsername
        [Fact]
        public async Task LoginWithUsername_Null_ReturnsStatusCode500()
        {
            // Arrange
            LoginWithUsernameDto loginDto = null;
            var errorMessage = "Object reference not set to an instance of an object.";

            // Act
            var actionResult = await _userController.LoginWithUsername(loginDto);

            // Assert
            var serverError = actionResult as ObjectResult;
            Assert.Equal(errorMessage, serverError.Value);
        }

        [Fact]
        public async Task LoginWithUsername_InvalidUserWithoutRequiredProperties_ReturnsStatusCode500()
        {
            // Arrange
            LoginWithUsernameDto loginDto = null;
            var errorMessage = "Value cannot be null. (Parameter 'value')";

            // Act
            var actionResult = await _userController.LoginWithUsername(loginDto);

            // Assert
            var serverError = actionResult as ObjectResult;
            Assert.Equal(errorMessage, serverError.Value);
        }

        [Fact]
        public async Task LoginWithUsername_ValidUser_ReturnsNewUserDto()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "validregister@g.c",
                UserName = "ValidUserRegister",
                Password = "!1Qqwertyuiop"
            };
            await _userController.Register(registerDto);

            var loginDto = new LoginWithUsernameDto
            {
                UserName = "ValidUserRegister",
                Password = "!1Qqwertyuiop"
            };

            // Act
            var actionResult = await _userController.LoginWithUsername(loginDto);

            // Assert
            var okResult = actionResult as ObjectResult;
            var newUserDto = okResult.Value as NewUserDto;
            Assert.NotNull(newUserDto);
        }
        #endregion
    }
}
