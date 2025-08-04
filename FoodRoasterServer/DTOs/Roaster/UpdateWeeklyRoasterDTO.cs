using System.ComponentModel.DataAnnotations;

namespace FoodRoasterServer.DTOs.Roaster
{
    public class UpdateWeeklyRoasterDTO
    {
        [Required]
        public DateTime MenuDate { get; set; }

        [Required]
        public bool IsVegChoice { get; set; }
    }
}
