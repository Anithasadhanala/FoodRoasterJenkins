using FoodRoasterServer.Constants;
using FoodRoasterServer.Data;
using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Exceptions.UserExceptions;
using FoodRoasterServer.Models;
using FoodRoasterServer.Services;
using FoodRoasterServer.Services.Interfaces;
using FoodRoasterServer.Types.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodRoasterServer.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;

        public AuthController(IAuthService authService, IUserService userService, AppDbContext context, IAuditService auditService)
        {
            _authService = authService;
            _userService = userService;
            _context = context;
            _auditService = auditService;
        }

        [HttpPost("login")]
        public async Task<UserLoginResponseDTO> Login([FromBody] UserDTO obj)
        {
            var response = await _authService.Login(obj);
            _auditService.Track($"User '{obj.Email}' logged in successfully.");
            return response;
        }


        [HttpPost("register")]
        public async Task<UserRegisterDTO> Register([FromBody] UserRegisterDTO obj)
        {
            // DTO validaitons
            var existingUser = await _context.Users
                 .FirstOrDefaultAsync(u => u.Email == obj.Email);

            if (existingUser != null) throw new DuplicateEmailException(AppMessages.Errors.UserEmailExists);
            if (!Enum.TryParse(typeof(RoleType), obj.Role, out var parsedRole) || !Enum.IsDefined(typeof(RoleType), parsedRole))
                throw new ArgumentException(AppMessages.Validation.UserRolesValidation);

            var registeredUser = await _userService.Register(obj);
            _auditService.Track($"New user '{obj.Email}' registered with role '{obj.Role}'.");
            return registeredUser;
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<string> Logout()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var res = await _authService.Logout(token);
            _auditService.Track($"User '{userEmail}' logged out successfully.");
            return res;

        }

    }
}
