
using FoodRoasterServer.Data.SeedData;
using Microsoft.EntityFrameworkCore;

namespace FoodRoasterServer.Data
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            _context.Database.Migrate();

            new UserSeeder(_context).Seed();
            new FoodItemsSeeder(_context).Seed();
            new HolidaysSeeder(_context).Seed();
        }
    }
}
