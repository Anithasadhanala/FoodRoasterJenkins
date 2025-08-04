using FoodRoasterServer.Constants;
using FoodRoasterServer.DTOs.Roaster;
using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodRoasterServer.Constants;
using FoodRoasterServer.Extensions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using FoodRoasterServer.Services.Interfaces;
using FoodRoasterServer.Models;

namespace FoodRoasterServer.Controllers
{

    [ApiController]
    [Route("api/roasters/")]
    public class RoasterController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRoasterService _roasterService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoasterController(IAuthService authService, IRoasterService roasterService, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _roasterService = roasterService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }


        [Authorize]
        [HttpPost("register")]
        public async Task<List<RoasterRegisterResponseDTO>> RegisterForRoaster([FromBody] RoasterRegisterDTO obj)
        {
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            var response = await _roasterService.RegisterToWeeklyRoaster(obj);
            _auditService.Track($"User '{userEmail}' registered for roaster.");
            return response;
        }



        [Authorize]
        [HttpGet("weekly")]
        public async Task<ActionResult<List<FoodMenu>>> GetWeeklyFoodRoaster([FromQuery] string period)
        {
            var (startDate, endDate) = _roasterService.ValidateGivenPeriod(period);
            return Ok(await _roasterService.GetWeeklyRoaster(startDate, endDate));
        }

        [Authorize]
        [HttpGet("weekly/user")]
        public async Task<List<WeeklyUserRoasterDTO>> GetUserWeeklyFoodRoaster([FromQuery] string period)
        {
            var (startDate, endDate) = _roasterService.ValidateGivenPeriod(period);
            return await _roasterService.GetUserWeeklyRoaster(startDate, endDate);
        }


        [Authorize]
        [HttpPut]
        public async Task<List<WeeklyUserRoasterDTO>> UpdateWeeklyFoodRoaster([FromBody] List<UpdateWeeklyRoasterDTO> reqBody)
        {

            _roasterService.ValidateForPassedDays(reqBody);
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            var updatedList = new List<WeeklyUserRoasterDTO>();
            foreach (var item in reqBody)
            {
                var res_entity = await _roasterService.UpdateWeeklyRoaster(item);
                if (res_entity != null)
                {
                    updatedList.Add(res_entity);
                }
            }
            _auditService.Track($"User '{userEmail}' updated weekly roaster.");
            return updatedList;
        }

        [Authorize]
        [HttpDelete]
        public async Task<string> DeleteWeeklyFoodRoaster([FromBody] List<DeleteWeeklyRoasterDTO> reqBody)
        {
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            foreach (var item in reqBody)
            {
                await _roasterService.RemoveWeeklyRoasterByDate(item);
                // handle loggers here.
            }
            _auditService.Track($"User '{userEmail}' deleted weekly roaster.");
            return AppMessages.Success.DeleteFoodRoasterSuccess; 

        }


        [Authorize(Roles = "ADMIN")]
        [HttpGet("registration_stats")]
        public async Task<ActionResult<List<DailyRoasterRegistrationStats>>> GetWeeklyStats([FromQuery] DateTime? weekStartDate)
        {
            var inputDate = weekStartDate ?? DateTime.Today;
            var startDate = inputDate.StartOfWeek(DayOfWeek.Monday);
            return await _roasterService.GetRoasterWeeklyStats(startDate);
        }
    }
}