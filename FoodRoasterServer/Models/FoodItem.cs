using FoodRoasterServer.Types.enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FoodRoasterServer.Models
{
    public class FoodItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [EnumDataType(typeof(FoodCategory))]
        public FoodCategory Category { get; set; }

        [Required]
        public bool IsVeg { get; set; }

        [JsonIgnore]
        public ICollection<FoodMenu> FoodMenus { get; set; }

    }
}
