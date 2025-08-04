using FoodRoasterServer.Controllers;
using FoodRoasterServer.Data;
using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Models;
using FoodRoasterServer.Services;
using FoodRoasterServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodRoasterServer.Repositories;
using Moq.EntityFrameworkCore;

namespace Tests.FoodRoasterServer.UnitTests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private Mock<IUserService> _mockUserService;
        private Mock<AppDbContext> _mockDbContext;
        private Mock<IAuditService> _mockAuditService;
        private AuthController _authController;

        [TestInitialize] // Inititlizing all the dependencies
        public void TestInitialize()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUserService = new Mock<IUserService>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockAuditService = new Mock<IAuditService>();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@example.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            httpContext.Request.Headers["Authorization"] = "Bearer test-token";

            _authController = new AuthController( // Mocking the DI of the authController
                _mockAuthService.Object,
                _mockUserService.Object,
                _mockDbContext.Object,
                _mockAuditService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [TestMethod] // Test method for the login controller method
        public async Task Login_WhenCalled_ReturnsUserLoginResponseDTO()
        {
            // Arrange
            var userDto = new UserDTO();
            var expectedResponse = new UserLoginResponseDTO();
            _mockAuthService.Setup(s => s.Login(userDto)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _authController.Login(userDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse, result);
            _mockAuditService.Verify(a => a.Track(It.IsAny<string>()), Times.Once);
        }
          
        [TestMethod] // Test method for the register controller method
        public async Task Register_WhenCalled_ReturnsUserRegisterDTO()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDTO { Email = "new@example.com", Role = "USER" };
            var users = new List<User>();
            _mockDbContext.Setup(c => c.Users).ReturnsDbSet(users);
            _mockUserService.Setup(s => s.Register(userRegisterDto)).ReturnsAsync(userRegisterDto);

            // Act
            var result = await _authController.Register(userRegisterDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRegisterDto, result);
            _mockAuditService.Verify(a => a.Track(It.IsAny<string>()), Times.Once);
        }

        [TestMethod] // Test method for the logout controller method
        public async Task Logout_WhenCalled_ReturnsSuccessMessage()
        {
            // Arrange
            _mockAuthService.Setup(s => s.Logout(It.IsAny<string>())).ReturnsAsync("Success");

            // Act
            var result = await _authController.Logout();

            // Assert
            Assert.IsNotNull(result);
            _mockAuditService.Verify(a => a.Track(It.IsAny<string>()), Times.Once);
        }
    }
}
