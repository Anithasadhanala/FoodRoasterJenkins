using FoodRoasterServer.DTOs.User;

namespace FoodRoasterServer.Services
{
    public interface IUserService
    {
        public Task<UserRegisterDTO> Register(UserRegisterDTO obj);
    }
}
