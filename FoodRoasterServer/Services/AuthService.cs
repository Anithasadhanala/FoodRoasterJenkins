using AutoMapper;
using FoodRoasterServer.Data;
using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Exceptions.UserExceptions;
using FoodRoasterServer.Models;
using FoodRoasterServer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodRoasterServer.Constants;
using FoodRoasterServer.Services.Interfaces;

namespace FoodRoasterServer.Services
{
    public class AuthService :IAuthService
    {
        private readonly IGenericRepository<User> _genericRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IGenericRepository<User> genericRepository, IMapper mapper, AppDbContext context,IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserLoginResponseDTO> Login(UserDTO obj)
        {
            var user = (await _genericRepository.GetAllRecords())
                  .FirstOrDefault(u => u.Email == obj.Email);

            if (user == null) throw new Exception(AppMessages.Errors.InvalidEmail);

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordDigest, obj.Password);
            if (result == PasswordVerificationResult.Failed) throw new Exception(AppMessages.Errors.InvalidPassword);

            var jwt = GenerateJwtToken(user);
            var res_entity = _mapper.Map<UserLoginResponseDTO>(user);

            res_entity.Jwt = jwt;
            return res_entity;
        }

        public string HashPassword(string plainPassword, User user = null)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(user ?? new User(), plainPassword);
        }


        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"]);
            var jti = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, jti)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        public async Task<string> Logout(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null) throw new JwtNotFound(AppMessages.Errors.JwtNotFound);
            
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiry = jwtToken.ValidTo;

            System.Security.Claims.ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Create and save UserBlacklist record
            var userBlacklist = new UserBlacklist
            {
                UserId = userId,
                Jti = jti,
                ExpiryDate = expiry
            };

            _context.UserBlacklists.Add(userBlacklist);
            await _context.SaveChangesAsync();
            return AppMessages.Success.LoginSuccess;
        }
    }
}
