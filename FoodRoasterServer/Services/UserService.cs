using AutoMapper;
using FoodRoasterServer.Constants;
using FoodRoasterServer.Data;
using FoodRoasterServer.DTOs;
using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Exceptions.UserExceptions;
using FoodRoasterServer.Models;
using FoodRoasterServer.Repositories;
using FoodRoasterServer.Types.enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace FoodRoasterServer.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _genericRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public UserService(IGenericRepository<User> genericRepository, IMapper mapper, AppDbContext context)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<UserRegisterDTO> Register(UserRegisterDTO obj)
        {
            var req_entity = _mapper.Map<User>(obj);

            // Hash the password 
            var passwordHasher = new PasswordHasher<User>();
            req_entity.PasswordDigest = passwordHasher.HashPassword(req_entity, obj.Password);
            
            var res_entity = await _genericRepository.AddRecord(req_entity);
            return _mapper.Map<UserRegisterDTO>(res_entity);
        }
    }
}
