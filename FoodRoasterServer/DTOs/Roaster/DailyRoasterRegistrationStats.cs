using System.Globalization;

namespace FoodRoasterServer.DTOs.Roaster
{
    public class DailyRoasterRegistrationStats
    {
        public DateTime Date { get; set; }
        public int TotalRegistrations { get; set; }
        public int VegCount { get; set; }
        public int NonVegCount { get; set; }

        public bool IsHoliday { get; set; }
        public String HolidayReason { get; set; }
    }
}
