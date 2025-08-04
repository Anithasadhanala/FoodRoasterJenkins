using AutoMapper;
using FoodRoasterServer.DTOs.Roaster;
using FoodRoasterServer.DTOs.User;
using FoodRoasterServer.Models;
using System.IO;


namespace FoodRoasterServer.Mappers
{
    public class MappingProfile :Profile
    {
        public MappingProfile() {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<UserLoginResponseDTO, User>().ReverseMap();
            //CreateMap<RoasterRegisterResponseDTO, UserMealRegistration>().ReverseMap();
            CreateMap<UserMealRegistration, RoasterRegisterResponseDTO>()
                .ForMember(dest => dest.FoodMenu, opt => opt.MapFrom(src => src.FoodMenu));
            CreateMap<WeeklyUserRoasterDTO, UserMealRegistration>().ReverseMap();
        }
    }
}
