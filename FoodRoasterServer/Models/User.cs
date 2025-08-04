using FoodRoasterServer.Types.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FoodRoasterServer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        
        public string PasswordDigest { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        //[EnumDataType(typeof(RoleType))]
        [EnumDataType(typeof(RoleType), ErrorMessage = "Invalid role specified.")]

        [Column(TypeName = "varchar(20)")]
        public RoleType Role { get; set; }

        public ICollection<UserBlacklist> UserBlacklists { get; set; }
        public ICollection<UserMealRegistration> UserMealRegistrations { get; set; }
    }
}
