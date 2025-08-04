using FoodRoasterServer.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodRoasterServer.DTOs.Roaster
{
    public class WeeklyUserRoasterDTO
    {
        public int Id { get; set; }
        public int FoodMenuId { get; set; }
        [Required]
        public DateTime RegisteredAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public bool IsVegChoice { get; set; }
        public FoodMenu FoodMenu { get; set; }
        public bool IsHoliday { get; set; }
        public string HolidayReason { get; set; }
    }

}
