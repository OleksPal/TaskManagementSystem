using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.UnitTests
{
    [Collection("TestCollection")]
    public class UserControllerTests
    {
        protected readonly UserController _userController;

        protected readonly User ExistingUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "ExistingUserControllerTest",
            Email = "exist@mail.com"
        };

        public UserControllerTests()
        {
            var userManagerMock = Helper.MockUserManager<User>([ExistingUser]);
            var tokenServiceMock = Helper.MockTokenService();
            var signInManagerMock = Helper.MockSignInManager(userManagerMock.Object);
            var loggerMock = new Mock<ILogger<UserController>>();

            var userControllerMock = new Mock<UserController>(userManagerMock.Object, tokenServiceMock.Object,
                signInManagerMock.Object, loggerMock.Object);            

            userControllerMock.CallBase = true;

            userControllerMock.Protected()
                .SetupSequence<Task<User>>("GetUserByEmail", ItExpr.IsAny<string>())
                .Returns(Task.FromResult(ExistingUser))
                .Returns(Task.FromResult<User>(null));

            userControllerMock.Protected()
                .SetupSequence<Task<User>>("GetUserByUsername", ItExpr.IsAny<string>())
                .Returns(Task.FromResult(ExistingUser))
                .Returns(Task.FromResult<User>(null));            

            _userController = userControllerMock.Object;

            var objectValidator = new TestingObjectValidator { Controller = _userController };
            _userController.ObjectValidator = objectValidator;
        }

        #region Register
        [Fact]
        public async Task Register_Null_ReturnsStatusCode500()
        {
            // Arrange
            RegisterDto registerDto = null;
            var errorMessage = "Value cannot be null. (Parameter 'model')";

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

            // Act
            var actionResult = await _userController.Register(registerDto);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            var errorsNumber = (badRequest.Value as SerializableError).Count;
            Assert.Equal(3, errorsNumber);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsNewUserDto()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = ExistingUser.Email,
                UserName = ExistingUser.UserName,
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
        public async Task LoginWithUsername_Null_ReturnsArgumentNullException()
        {
            // Arrange
            LoginWithUsernameDto loginDto = null;

            // Act
            Func<Task> act = () => _userController.LoginWithUsername(loginDto);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task LoginWithUsername_InvalidUserWithoutRequiredProperties_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var loginDto = new LoginWithUsernameDto();

            // Act
            var actionResult = await _userController.LoginWithUsername(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginWithUsername_ValidUser_UserExists_ReturnsNewUserDto()
        {
            // Arrange
            var loginDto = new LoginWithUsernameDto
            {
                UserName = ExistingUser.UserName,
                Password = "!1Qqwertyuiop"
            };

            // Act
            var actionResult = await _userController.LoginWithUsername(loginDto);

            // Assert
            var okResult = actionResult as ObjectResult;
            var newUserDto = okResult.Value as NewUserDto;
            Assert.NotNull(newUserDto);
        }

        [Fact]
        public async Task LoginWithUsername_ValidUser_UserDoesNotExists_ReturnsNewUserDto()
        {
            // Arrange
            var loginDto = new LoginWithUsernameDto
            {
                UserName = ExistingUser.UserName,
                Password = "!1Qqwertyuiop"
            };

            // Act
            await _userController.LoginWithUsername(loginDto);
            // Calling a method a second time so that the method is called by a non-existent user
            var actionResult = await _userController.LoginWithUsername(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult);
        }
        #endregion
    }
}
