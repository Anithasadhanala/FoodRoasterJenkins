using System.ComponentModel.DataAnnotations;

namespace FoodRoasterServer.Models
{
    public class Holiday
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime HolidayDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; }
    }
}
