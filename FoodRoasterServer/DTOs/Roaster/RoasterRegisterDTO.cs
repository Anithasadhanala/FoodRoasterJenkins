using System.ComponentModel.DataAnnotations;

namespace FoodRoasterServer.DTOs.Roaster
{
    public class RoasterRegisterDTO
    {

        [Required]
        public List<MealRegistrationDTO> Meals { get; set; }
    }
    public class MealRegistrationDTO
    {
        [Required]
        public int FoodMenuId { get; set; }

        [Required]
        public bool IsVegChoice { get; set; }
    }
}
