using System.ComponentModel.DataAnnotations;

namespace FoodRoasterServer.DTOs.Roaster
{
    public class DeleteWeeklyRoasterDTO
    {
        [Required]
        public DateTime MenuDate { get; set; }
    }
}
