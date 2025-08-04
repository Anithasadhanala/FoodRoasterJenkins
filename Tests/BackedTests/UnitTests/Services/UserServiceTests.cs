using AutoMapper;
using FoodRoasterServer.Data;
using FoodRoasterServer.Services;
using Moq;
using FoodRoasterServer.Models;
using Microsoft.EntityFrameworkCore;
using FoodRoasterServer.DTOs.User;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodRoasterServer.Repositories;

namespace Tests.FoodRoasterServer.UnitTests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IGenericRepository<User>> _mockGenericRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<AppDbContext> _mockDbContext;
        private UserService _userService;

        [TestInitialize] // Initializing the depencies 
        public void TestInitialize()
        {
            _mockGenericRepository = new Mock<IGenericRepository<User>>();
            _mockMapper = new Mock<IMapper>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

            _userService = new UserService(   // Mocking the DI here
                _mockGenericRepository.Object,
                _mockMapper.Object,
                _mockDbContext.Object
            );
        }

        [TestMethod] // Test method for registering the new user
        public async Task Register_WhenCalled_ReturnsUserRegisterDTO()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDTO { Email = "test@example.com", Password = "password" };
            var user = new User { Id = 1, Email = "test@example.com" };

            _mockMapper.Setup(m => m.Map<User>(userRegisterDto)).Returns(user);
            _mockGenericRepository.Setup(r => r.AddRecord(user)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserRegisterDTO>(user)).Returns(userRegisterDto);

            // Act
            var result = await _userService.Register(userRegisterDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRegisterDto.Email, result.Email);
            _mockGenericRepository.Verify(r => r.AddRecord(It.IsAny<User>()), Times.Once);
        }
    }
}