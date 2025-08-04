using FoodRoasterServer.Models;

namespace FoodRoasterServer.Data.SeedData
{
    public class HolidaysSeeder
    {
        private readonly AppDbContext _context;

        public HolidaysSeeder(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.Holidays.Any())
            {
                var holidays = new List<Holiday>
                {
                    new Holiday { HolidayDate = new DateTime(2025, 1, 1), Reason = "New Year's Day" },
                    new Holiday { HolidayDate = new DateTime(2025, 1, 26), Reason = "Republic Day" },
                    new Holiday { HolidayDate = new DateTime(2025, 8, 15), Reason = "Independence Day" },
                    new Holiday { HolidayDate = new DateTime(2025, 10, 2), Reason = "Gandhi Jayanti" },
                    new Holiday { HolidayDate = new DateTime(2025, 12, 25), Reason = "Christmas Day" }
                };

                _context.Holidays.AddRange(holidays);
                _context.SaveChanges();
            }
        }
    }
}
