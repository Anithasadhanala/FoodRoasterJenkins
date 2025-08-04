using FoodRoasterServer.Types.enums;
using System.ComponentModel.DataAnnotations;

public class WeeklyMenuCreateDTO
{
    [Required]
    public DateTime MenuDate { get; set; }
    [Required]
    public List<FoodItemCreateDTO> FoodItems { get; set; }
}

public class FoodItemCreateDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    public FoodCategory Category { get; set; }

    [Required]
    public bool IsVeg { get; set; }
}
