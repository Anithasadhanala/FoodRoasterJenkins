using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using FoodRoasterServer.Types.enums;

namespace FoodRoasterServer.DTOs.User
{
    public class UserRegisterDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        [EnumDataType(typeof(RoleType), ErrorMessage = "Invalid role specified.")]
        public string Role { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Password { get; set; }
    }
}