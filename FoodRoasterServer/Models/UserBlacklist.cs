using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodRoasterServer.Models
{
    public class UserBlacklist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Jti { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public User User { get; set; }
    }
}
