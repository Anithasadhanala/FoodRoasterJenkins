using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FoodRoasterServer.Models
{
    public class FoodMenu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime MenuDate { get; set; }
        public ICollection<FoodItem> FoodItems { get; set; }
        [JsonIgnore]
        public ICollection<UserMealRegistration> UserMealRegistrations { get; set; }
    }
}
