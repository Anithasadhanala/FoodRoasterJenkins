using FoodRoasterServer.Data;
using System;
using System.Linq;

namespace FoodRoasterServer.BackgroundJobs
{
    public class TokenCleanUpJob
    {
        private readonly AppDbContext _context;

        public TokenCleanUpJob(AppDbContext context)
        {
            _context = context;
        }

        // Removes the expired token from DB
        public void RemoveExpiredTokens()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _context.UserBlacklists
                .Where(t => t.ExpiryDate < now)
                .ToList();

            if (expiredTokens.Any())
            {
                _context.UserBlacklists.RemoveRange(expiredTokens);
                _context.SaveChanges();
            }
        }
    }
}
