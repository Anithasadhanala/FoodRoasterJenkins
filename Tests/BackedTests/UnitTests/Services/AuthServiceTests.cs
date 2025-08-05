using AutoMapper;
using FoodRoasterServer.Data;
using FoodRoasterServer.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using FoodRoasterServer.Models;
using Microsoft.EntityFrameworkCore;
using FoodRoasterServer.DTOs.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodRoasterServer.Repositories;
using Moq.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tests.FoodRoasterServer.UnitTests.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IGenericRepository<User>> _mockGenericRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<AppDbContext> _mockDbContext;
        private Mock<IConfiguration> _mockConfiguration;
        private AuthService _authService;

        [TestInitialize] // Initialising the depencies
        public void TestInitialize()
        {
            _mockGenericRepository = new Mock<IGenericRepository<User>>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>(), _mockConfiguration.Object);

            // Setup user claims -> this is for mocking the depencies
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _mockConfiguration.Setup(c => c["JwtSettings:Key"]).Returns("a-super-secret-key-that-is-long-enough-for-hs256");

            _authService = new AuthService( // Mocking the DI of the authService
                _mockGenericRepository.Object,
                _mockMapper.Object,
                _mockDbContext.Object,
                _mockConfiguration.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [TestMethod] // Test method for Login the user
        public async Task Login_WhenCalled_ReturnsUserLoginResponseDTO()
        {
            // Arrange
            var userDto = new UserDTO { Email = "test@example.com", Password = "password" };
            var user = new User { Id = 1, Email = "test@example.com", PasswordDigest = _authService.HashPassword("password") };
            _mockGenericRepository.Setup(r => r.GetAllRecords()).ReturnsAsync(new List<User> { user });
            _mockMapper.Setup(m => m.Map<UserLoginResponseDTO>(user)).Returns(new UserLoginResponseDTO { Email = user.Email });

            // Act
            var result = await _authService.Login(userDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.IsNotNull(result.Jwt);
        }

        [TestMethod] // Test method for  logout of the authorised used
        public async Task Logout_WhenCalled_AddsTokenToBlacklist()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            var token = _authService.GenerateJwtToken(user);
            _mockDbContext.Setup(c => c.UserBlacklists).ReturnsDbSet(new List<UserBlacklist>());
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.Logout(token);

            // Assert
            _mockDbContext.Verify(c => c.UserBlacklists.Add(It.IsAny<UserBlacklist>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.IsNotNull(result);
        }
    }
}