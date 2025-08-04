using FoodRoasterServer.Controllers;
using FoodRoasterServer.DTOs.Roaster;
using FoodRoasterServer.Services;
using FoodRoasterServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.FoodRoasterServer.UnitTests.Controllers
{
    [TestClass]
    public class RoasterControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private Mock<IRoasterService> _mockRoasterService;
        private Mock<IAuditService> _mockAuditService;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private RoasterController _roasterController;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockRoasterService = new Mock<IRoasterService>();
            _mockAuditService = new Mock<IAuditService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@example.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _roasterController = new RoasterController(
                _mockAuthService.Object,
                _mockRoasterService.Object,
                _mockAuditService.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [TestMethod]
        public async Task RegisterForRoaster_WhenCalled_ReturnsListOfRoasterRegisterResponseDTO()
        {
            // Arrange
            var roasterRegisterDto = new RoasterRegisterDTO();
            var expectedResponse = new List<RoasterRegisterResponseDTO>();
            _mockRoasterService.Setup(s => s.RegisterToWeeklyRoaster(roasterRegisterDto)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _roasterController.RegisterForRoaster(roasterRegisterDto);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expectedResponse, result);
            _mockAuditService.Verify(a => a.Track(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetWeeklyFoodRoaster_WhenCalled_ReturnsListOfWeeklyUserRoasterDTO()
        {
            // Arrange
            var period = "current-week";
            var expectedResponse = new List<WeeklyUserRoasterDTO>();
            _mockRoasterService.Setup(s => s.GetUserWeeklyRoaster(It.IsAny<System.DateTime>(), It.IsAny<System.DateTime>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _roasterController.GetWeeklyFoodRoaster(period);

            // Assert
            Assert.IsNotNull(result);
            //CollectionAssert.AreEqual(expectedResponse, result);
        }

        [TestMethod]
        public async Task UpdateWeeklyFoodRoaster_WhenCalled_ReturnsListOfWeeklyUserRoasterDTO()
        {
            // Arrange
            var updateDto = new List<UpdateWeeklyRoasterDTO>();
            var expectedResponse = new List<WeeklyUserRoasterDTO>();
            _mockRoasterService.Setup(s => s.UpdateWeeklyRoaster(It.IsAny<UpdateWeeklyRoasterDTO>())).ReturnsAsync(new WeeklyUserRoasterDTO());

            // Act
            var result = await _roasterController.UpdateWeeklyFoodRoaster(updateDto);

            // Assert
            Assert.IsNotNull(result);
            _mockAuditService.Verify(a => a.Track(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteWeeklyFoodRoaster_WhenCalled_ReturnsSuccessMessage()
        {
            // Arrange
            var deleteDto = new List<DeleteWeeklyRoasterDTO>();

            // Act
            var result = await _roasterController.DeleteWeeklyFoodRoaster(deleteDto);

            // Assert
            Assert.IsNotNull(result);
            _mockAuditService.Verify(a => a.Track(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetWeeklyStats_WhenCalled_ReturnsListOfDailyRoasterRegistrationStats()
        {
            // Arrange
            var expectedResponse = new List<DailyRoasterRegistrationStats>();
            _mockRoasterService.Setup(s => s.GetRoasterWeeklyStats(It.IsAny<System.DateTime>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _roasterController.GetWeeklyStats(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<List<DailyRoasterRegistrationStats>>));
        }
    }
}