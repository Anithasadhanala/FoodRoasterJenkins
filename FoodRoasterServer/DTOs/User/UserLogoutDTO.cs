using FoodRoasterServer.Types.enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodRoasterServer.DTOs.User
{
    public class UserLogoutDTO
    {
        public int Id { get; set; }
        public string? Jwt { get; set; }
    }
}
