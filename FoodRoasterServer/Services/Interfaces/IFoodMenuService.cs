using FoodRoasterServer.DTOs.Menu;

namespace FoodRoasterServer.Services
{
    public interface IFoodMenuService
    {
        public Task<List<FoodMenuDTO>> GetMenuByDate(DateTime date);
        //public void CreateMenuForTheDay();
        //public void UpdateFoodMenuByDate();
        //public void DeleteFoodMenuByDate();
    }
}
