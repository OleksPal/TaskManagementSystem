using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Security.Claims;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Models;
using TaskManagementSystem.UnitTests.Extensions;

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

            _userController.InitializeClaims(new Claim(ClaimTypes.GivenName, ExistingUser.UserName));
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
        public async Task Register_InvalidUserWithoutRequiredProperties_ReturnsBadRequestObjectResult()
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

        #region Login
        [Fact]
        public async Task Login_Null_ReturnsArgumentNullException()
        {
            // Arrange
            LoginDto loginDto = null;

            // Act
            Func<Task> act = () => _userController.Login(loginDto);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Login_InvalidUserWithoutRequiredProperties_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var loginDto = new LoginDto();

            // Act
            var actionResult = await _userController.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Login_ValidUser_UserExists_ReturnsNewUserDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Login = ExistingUser.UserName,
                Password = "!1Qqwertyuiop"
            };

            // Act
            var actionResult = await _userController.Login(loginDto);

            // Assert
            var okResult = actionResult as ObjectResult;
            var newUserDto = okResult.Value as NewUserDto;
            Assert.NotNull(newUserDto);
        }

        [Fact]
        public async Task Login_ValidUserName_UserDoesNotExists_ReturnsNewUserDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Login = ExistingUser.UserName,
                Password = "!1Qqwertyuiop"
            };

            // Act
            await _userController.Login(loginDto);
            // Calling a method a second time so that the method is called by a non-existent user
            var actionResult = await _userController.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult);
        }

        [Fact]
        public async Task Login_ValidUserEmail_UserExists_ReturnsNewUserDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Login = ExistingUser.Email,
                Password = "!1Qqwertyuiop"
            };

            // Act
            var actionResult = await _userController.Login(loginDto);

            // Assert
            var okResult = actionResult as ObjectResult;
            var newUserDto = okResult.Value as NewUserDto;
            Assert.NotNull(newUserDto);
        }
        #endregion

        #region ChangePassword
        [Fact]
        public async Task ChangePassword_Null_ReturnsArgumentNullException()
        {
            // Arrange
            ChangePasswordDto changePasswordDto = null;

            // Act
            Func<Task> act = () => _userController.ChangePassword(changePasswordDto);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task ChangePassword_InvalidUserWithoutRequiredProperties_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto();

            // Act
            var actionResult = await _userController.ChangePassword(changePasswordDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task ChangePassword_PasswordDontMatch_ReturnsOkResult()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "!1Qqwertyuiop",
                NewPassword = "!1Qqwertyuiop",
                ConfirmPassword = "!2Qqwertyuiop"
            };
            var expectedErrorMessage = "The new password and confirmation password do not match.";

            // Act
            var actionResult = await _userController.ChangePassword(changePasswordDto);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            var errorMessages = (badRequest.Value as SerializableError).Values.FirstOrDefault();
            var passwordErrorMessage = (errorMessages as string[]).FirstOrDefault();
            Assert.Equal(expectedErrorMessage, passwordErrorMessage);
        }

        [Fact]
        public async Task ChangePassword_ValidDto_UserExists_ReturnsOkResult()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "!1Qqwertyuiop",
                NewPassword = "!2Qqwertyuiop",
                ConfirmPassword = "!2Qqwertyuiop"
            };

            // Act
            var actionResult = await _userController.ChangePassword(changePasswordDto);

            // Assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task ChangePassword_ValidDto_UserDoesNotExists_ReturnsStatusCode500()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "!1Qqwertyuiop",
                NewPassword = "!2Qqwertyuiop",
                ConfirmPassword = "!2Qqwertyuiop"
            };

            // Act
            await _userController.ChangePassword(changePasswordDto);
            // Calling a method a second time so that the method is called by a non-existent user
            var actionResult = await _userController.ChangePassword(changePasswordDto);

            // Assert
            var serverError = actionResult as StatusCodeResult;
            Assert.Equal(500, serverError.StatusCode);
        }
        #endregion
    }
}
