using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FoodRoasterServer.Types.enums;

namespace FoodRoasterServer.DTOs.Menu
{
    public class FoodMenuDTO
    {
        [Required]
        public DateTime MenuDate { get; set; }

        [Required]
        public List<FoodItemDTO> FoodItems { get; set; }
    }

    public class FoodItemDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public FoodCategory Category { get; set; }

        public bool IsVeg { get; set; }
    }
}
