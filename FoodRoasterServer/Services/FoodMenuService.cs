using AutoMapper;
using FoodRoasterServer.Data;
using FoodRoasterServer.DTOs.Menu;
using FoodRoasterServer.Models;
using FoodRoasterServer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodRoasterServer.Services
{
    public class FoodMenuService : IFoodMenuService
    {

        private readonly IGenericRepository<FoodMenu> _genericRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public FoodMenuService(IGenericRepository<FoodMenu> genericRepository, IMapper mapper, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<FoodMenuDTO>> GetMenuByDate(DateTime date)
        {
            var menus = await _context.FoodMenus
               .Include(m => m.FoodItems)
               .Where(m => m.MenuDate.Date == date)
               .ToListAsync();

            return menus.Select(menu => new FoodMenuDTO
            {
                MenuDate = menu.MenuDate,
                FoodItems = menu.FoodItems.Select(item => new FoodItemDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category,
                    IsVeg = item.IsVeg
                }).ToList()
            }).ToList();
        }
       
    }
}
    