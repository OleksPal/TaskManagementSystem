using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using TaskManagementSystem.Services.Interfaces;

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
        #endregion
    }
}
