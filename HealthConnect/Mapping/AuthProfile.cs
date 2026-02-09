using AutoMapper;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            // DTO <-> Domain with ReverseMap
            CreateMap<SignupRequestDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<LoginRequestDto, User>().ReverseMap();

            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
