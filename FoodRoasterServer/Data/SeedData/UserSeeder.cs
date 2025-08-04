using FoodRoasterServer.Models;
using FoodRoasterServer.Types.enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
using FoodRoasterServer.Types.enums;

namespace FoodRoasterServer.Data.SeedData
{
    public class UserSeeder
    {
        private readonly AppDbContext _context;

        public UserSeeder(AppDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.Users.Any(u => u.Email == "anitha@gmail.com"))
            {
                var user = new User
                {
                    UserName = "Anitha",
                    Email = "anitha@gmail.com",
                    Role = RoleType.ADMIN,
                    PasswordDigest = "AQAAAAIAAYagAAAAEIBYhGN4I94zSBf+oLc8Pa9nL1z+iW/y9oVRNeSt8+bh3Y2Be4BXClTypP+u8JwcBg==" // test@123
                };

                _context.Users.Add(user);
                _context.SaveChanges();
            }
        }
    }
}
