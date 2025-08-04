using FoodRoasterServer.DTOs.Menu;
using FoodRoasterServer.DTOs.Roaster;
using FoodRoasterServer.Models;
using FoodRoasterServer.Services;
using FoodRoasterServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodRoasterServer.Controllers
{
    [ApiController]
    [Route("api/menu/")]
    public class MenuController
    {
        private readonly IAuthService _authService;
        private readonly IFoodMenuService _foodMenuService;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public MenuController(IAuthService authService, IFoodMenuService foodMenuService, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _foodMenuService = foodMenuService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<List<FoodMenuDTO>> GetMenuByDate([FromQuery] DateTime? menuDate)
        {

            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            var date = menuDate?.Date ?? DateTime.Today;
            var menu = await _foodMenuService.GetMenuByDate(date);
            _auditService.Track($"User '{userEmail}' requested menu for date '{date}'.");
            return menu;
        }
    }
}
