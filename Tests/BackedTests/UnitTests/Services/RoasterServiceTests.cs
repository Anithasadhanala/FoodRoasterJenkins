using AutoMapper;
using FoodRoasterServer.Data;
using FoodRoasterServer.Services;
using FoodRoasterServer.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using FoodRoasterServer.Models;
using Microsoft.EntityFrameworkCore;
using FoodRoasterServer.DTOs.Roaster;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoodRoasterServer.Repositories;
using Moq.EntityFrameworkCore;
using FoodRoasterServer.Constants;

namespace Tests.FoodRoasterServer.UnitTests.Services
{
    [TestClass]
    public class RoasterServiceTests
    {
        private Mock<IGenericRepository<UserMealRegistration>> _mocGenericRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<AppDbContext> _mockDbContext;
        private RoasterService _roasterService;

        [TestInitialize]
        public void TestInitialize() // Here initializations required are mentioned here
        {
            _mockGenericRepository = new Mock<IGenericRepository<UserMealRegistration>>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
             
            // Setup user claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _roasterService = new RoasterService(    // Mocking the DI here
                _mockGenericRepository.Object,
                _mockMapper.Object,
                _mockDbContext.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [TestMethod] // Test method for registering weekly roaster
        public async Task RegisterToWeeklyRoaster_WhenCalled_ReturnsRegisteredMeals()
        {
            // Arrange
            var roasterRegisterDto = new RoasterRegisterDTO
            {
                Meals = new List<MealRegistrationDTO>
                {
                    new MealRegistrationDTO { FoodMenuId = 1, IsVegChoice = true }
                }
            };
            var userMealRegistrations = new List<UserMealRegistration>();    
            _mockDbContext.Setup(c => c.UserMealRegistrations).ReturnsDbSet(userMealRegistrations);
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            var responseDto = new List<RoasterRegisterResponseDTO>
            {
                new RoasterRegisterResponseDTO { FoodMenuId = 1, IsVegChoice = true }
            };
            _mockMapper.Setup(m => m.Map<List<RoasterRegisterResponseDTO>>(It.IsAny<List<UserMealRegistration>>())).Returns(responseDto);

            // Act
            var result = await _roasterService.RegisterToWeeklyRoaster(roasterRegisterDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].FoodMenuId);
        }

        [TestMethod] // Test method for registering weekly roaster when user is already registered
        public async Task RegisterToWeeklyRoaster_UserAlreadyRegistered_ThrowsUseRegisteredForFoodException()
        {
            // Arrange
            var roasterRegisterDto = new RoasterRegisterDTO
            {
                Meals = new List<MealRegistrationDTO>
                {
                    new MealRegistrationDTO { FoodMenuId = 1, IsVegChoice = true }
                }
            };

            var userMealRegistrations = new List<UserMealRegistration>();
            _mockDbContext.Setup(c => c.UserMealRegistrations).ReturnsDbSet(userMealRegistrations);
            var dbUpdateException = new DbUpdateException("Error", new Exception("Violation of unique constraint 'UX_UserMealRegistration_UserId_FoodMenuId'"));
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(dbUpdateException);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UseRegisteredForFoodException>(() =>
                _roasterService.RegisterToWeeklyRoaster(roasterRegisterDto));
        }

        [TestMethod] // Test method for ValidateGivenPeriod with invalid period
        public void ValidateGivenPeriod_InvalidPeriod_ThrowsArgumentException()
        {
            // Arrange
            var invalidPeriod = "invalid-period";

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentException>(() =>
                _roasterService.ValidateGivenPeriod(invalidPeriod));

            Assert.AreEqual(AppMessages.Errors.InvalidRoasterQueryParams, exception.Message);
        }

        [TestMethod] // Test method for ValidateForPassedDays with past date
        public void ValidateForPassedDays_PastDate_ThrowsInvalidOperationException()
        {
            // Arrange
            var pastDate = DateTime.Today.AddDays(-1);
            var reqBody = new List<UpdateWeeklyRoasterDTO>
            {
                new UpdateWeeklyRoasterDTO { MenuDate = pastDate, IsVegChoice = true }
            };

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                _roasterService.ValidateForPassedDays(reqBody));

            Assert.AreEqual(AppMessages.Errors.InvalidDateModifications, exception.Message);
        }

        [TestMethod] // Test method for ValidateForPassedDays with current date past 9 AM
        public void ValidateForPassedDays_CurrentDatePast9AM_ThrowsInvalidOperationException()
        {
            // Arrange
            var currentTime = DateTime.Now;
            var nineAMToday = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 9, 0, 0);

            // Ensure the test runs only if current time is past 9 AM
            if (currentTime.TimeOfDay <= nineAMToday.TimeOfDay)
            {
                Assert.Inconclusive("Test can only run after 9 AM today.");
            }

            var reqBody = new List<UpdateWeeklyRoasterDTO>
            {
                new UpdateWeeklyRoasterDTO { MenuDate = currentTime, IsVegChoice = true }
            };

            // Act & Assert
            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                _roasterService.ValidateForPassedDays(reqBody));

            Assert.AreEqual(AppMessages.Errors.InvalidDateModifications, exception.Message);
        }

        [TestMethod] // Test method for Fetching weekly roaster
        public async Task GetUserWeeklyRoaster_WhenCalled_ReturnsWeeklyRoaster()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(6);
            var userId = 1;

            var registrations = new List<UserMealRegistration>
            {
                new UserMealRegistration { UserId = userId, FoodMenu = new FoodMenu { MenuDate = startDate } }
            };
            _mockDbContext.Setup(c => c.UserMealRegistrations).ReturnsDbSet(registrations);
            var weeklyRoasterDto = new List<WeeklyUserRoasterDTO>
            {
                new WeeklyUserRoasterDTO { RegisteredAt = startDate }
            };
            _mockMapper.Setup(m => m.Map<List<WeeklyUserRoasterDTO>>(It.IsAny<List<UserMealRegistration>>())).Returns(weeklyRoasterDto);
            _mockDbContext.Setup(c => c.Holidays).ReturnsDbSet(new List<Holiday>());

            // Act
            var result = await _roasterService.GetUserWeeklyRoaster(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count); // 7
        }

        //[TestMethod] // Test method for the updating currently login user
        //public async Task UpdateWeeklyRoaster_WhenCalled_ReturnsUpdatedRoaster()
        //{
        //    // Arrange
        //    var updateDto = new UpdateWeeklyRoasterDTO
        //    {
        //        MenuDate = DateTime.UtcNow.Date,
        //        IsVegChoice = true
        //    };

        //    var foodMenu = new FoodMenu { Id = 1, MenuDate = updateDto.MenuDate };
        //    _mockDbContext.Setup(c => c.FoodMenus).ReturnsDbSet(new List<FoodMenu> { foodMenu });
        //    _mockDbContext.Setup(c => c.UserMealRegistrations).ReturnsDbSet(new List<UserMealRegistration>());
        //    _mockDbContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        //    // Act
        //    var result = await _roasterService.UpdateWeeklyRoaster(updateDto);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.FoodMenuId);
        //}

        [TestMethod] // Test method for removing roaster
        public async Task RemoveWeeklyRoasterByDate_WhenCalled_RemovesRoaster()
        {
            // Arrange
            var deleteDto = new DeleteWeeklyRoasterDTO
            {
                MenuDate = DateTime.UtcNow.Date
            };
            var userId = 1;
            var registrations = new List<UserMealRegistration>
            {
                new UserMealRegistration { UserId = userId, FoodMenu = new FoodMenu { MenuDate = deleteDto.MenuDate } }
            };
            _mockDbContext.Setup(c => c.UserMealRegistrations).ReturnsDbSet(registrations);
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _roasterService.RemoveWeeklyRoasterByDate(deleteDto);

            // Assert
            _mockDbContext.Verify(c => c.UserMealRegistrations.RemoveRange(It.IsAny<List<UserMealRegistration>>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod] // Test method for fetching the Weekly roaster stats
        public async Task GetRoasterWeeklyStats_WhenCalled_ReturnsWeeklyStats()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date;
            var registrations = new List<UserMealRegistration>
            {
                new UserMealRegistration { FoodMenu = new FoodMenu { MenuDate = startDate }, IsVegChoice = true }
            };
            _mockDbContext.Setup(c => c.UserMealRegistrations).ReturnsDbSet(registrations);
            _mockDbContext.Setup(c => c.Holidays).ReturnsDbSet(new List<Holiday>());

            // Act
            var result = await _roasterService.GetRoasterWeeklyStats(startDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual(1, result[0].TotalRegistrations);
            Assert.AreEqual(1, result[0].VegCount);
        }
    }
}