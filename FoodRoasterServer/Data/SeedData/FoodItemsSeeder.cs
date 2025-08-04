using FoodRoasterServer.Models;
using FoodRoasterServer.Types.enums;

namespace FoodRoasterServer.Data.SeedData
{
    public class FoodItemsSeeder
    {
        private readonly AppDbContext _context;

        public FoodItemsSeeder(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.FoodItems.Any())
            {
                var foodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Paneer Tikka", Category = FoodCategory.STARTER, IsVeg = true },
                    new FoodItem { Name = "Chicken Biryani", Category = FoodCategory.MAIN_COURSE, IsVeg = false },
                    new FoodItem { Name = "Gulab Jamun", Category = FoodCategory.DESSERT, IsVeg = true },
                    new FoodItem { Name = "Masala Chai", Category = FoodCategory.BEVERAGE, IsVeg = true },
                    new FoodItem { Name = "Samosa", Category = FoodCategory.SNACK, IsVeg = true },
                    new FoodItem { Name = "Boiled Egg", Category = FoodCategory.OTHER, IsVeg = false },
                    new FoodItem { Name = "Vegetable Spring Rolls", Category = FoodCategory.SNACK, IsVeg = true },
                    new FoodItem { Name = "Butter Naan", Category = FoodCategory.MAIN_COURSE, IsVeg = true },
                    new FoodItem { Name = "Mutton Rogan Josh", Category = FoodCategory.MAIN_COURSE, IsVeg = false },
                    new FoodItem { Name = "Fruit Salad", Category = FoodCategory.DESSERT, IsVeg = true },
                    new FoodItem { Name = "Dal Makhani", Category = FoodCategory.MAIN_COURSE, IsVeg = true },
                    new FoodItem { Name = "Fish Curry", Category = FoodCategory.MAIN_COURSE, IsVeg = false },
                    new FoodItem { Name = "Idli", Category = FoodCategory.STARTER, IsVeg = true },
                    new FoodItem { Name = "Chicken Wings", Category = FoodCategory.STARTER, IsVeg = false },
                    new FoodItem { Name = "Lassi", Category = FoodCategory.BEVERAGE, IsVeg = true },
                    new FoodItem { Name = "Paneer Butter Masala", Category = FoodCategory.MAIN_COURSE, IsVeg = true },
                    new FoodItem { Name = "Egg Omelette", Category = FoodCategory.STARTER, IsVeg = false },
                    new FoodItem { Name = "Jalebi", Category = FoodCategory.DESSERT, IsVeg = true },
                    new FoodItem { Name = "Coffee", Category = FoodCategory.BEVERAGE, IsVeg = true },
                    new FoodItem { Name = "Mixed Veg Soup", Category = FoodCategory.OTHER, IsVeg = true }
                };

                _context.FoodItems.AddRange(foodItems);
                _context.SaveChanges();
            }
        }
    }
}
