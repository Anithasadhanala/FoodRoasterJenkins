using FoodRoasterServer.Models;
using FoodRoasterServer.Types;
using FoodRoasterServer.Types.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodRoasterServer.Data
{
    public class AppDbContext : DbContext

    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options) {
            _configuration = configuration;
        }

        public AppDbContext()
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserBlacklist> UserBlacklists { get; set; }
        public virtual DbSet<FoodItem> FoodItems  { get; set; }
        public virtual DbSet<FoodMenu> FoodMenus{ get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }

        public virtual DbSet<UserMealRegistration> UserMealRegistrations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);

            // category enum constraint
            modelBuilder.Entity<FoodItem>()
                .HasCheckConstraint("CK_FoodItems_Category_Enum",
                    $"Category IN ({string.Join(", ", Enum.GetValues(typeof(FoodCategory)).Cast<int>())})");

            modelBuilder.Entity<FoodMenu>()
                .HasMany(f => f.FoodItems)
                .WithMany(f => f.FoodMenus)
                .UsingEntity<Dictionary<string, object>>(
                    "FoodMenuItems",
                    r => r.HasOne<FoodItem>()
                          .WithMany()
                          .HasForeignKey("FoodItemsId")
                          .OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne<FoodMenu>()
                          .WithMany()
                          .HasForeignKey("FoodMenusId")
                          .OnDelete(DeleteBehavior.Cascade) // Cascade only from FoodMenu
                );

            // UserMealRegistration uniqu Index
            modelBuilder.Entity<UserMealRegistration>()
                .HasIndex(um => new { um.UserId, um.FoodMenuId })
                .IsUnique()
                .HasDatabaseName("UX_UserMealRegistration_UserId_FoodMenuId");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}