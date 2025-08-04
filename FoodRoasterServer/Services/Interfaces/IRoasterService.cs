using FoodRoasterServer.DTOs.Roaster;
using FoodRoasterServer.Models;
using System.Security.Claims;

namespace FoodRoasterServer.Services
{
    public interface IRoasterService
    {
        public Task<List<RoasterRegisterResponseDTO>> RegisterToWeeklyRoaster(RoasterRegisterDTO obj);
        public Task<List<WeeklyUserRoasterDTO>> GetUserWeeklyRoaster(DateTime startDate, DateTime endDate);
        public Task<List<FoodMenu>> GetWeeklyRoaster(DateTime startDate, DateTime endDate);

        public Task<WeeklyUserRoasterDTO> UpdateWeeklyRoaster(UpdateWeeklyRoasterDTO reqObj);
        public Task RemoveWeeklyRoasterByDate(DeleteWeeklyRoasterDTO reqObj);

        public Task<List<DailyRoasterRegistrationStats>> GetRoasterWeeklyStats(DateTime date);

        public (DateTime StartDate, DateTime EndDate) ValidateGivenPeriod(string period);
        public void ValidateForPassedDays(List<UpdateWeeklyRoasterDTO> reqBody);


    }
}
