using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Models;

namespace FoodRoasterServer.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<UserLoginResponseDTO> Login(UserDTO obj);
        public string HashPassword(string plainPassword, User user = null);
        public Task<string> Logout(string token);
    }
}
