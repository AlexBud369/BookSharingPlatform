using AutoMapper;
using Domain.Entities;
using Application.DTOs.User;

namespace Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile() 
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.userId));
        CreateMap<RegisterDto, User>()
            .ConvertUsing(src => new User(src.Username, src.Password, src.Email));

        CreateMap<LoginDto, User>()
            .ForMember(dest => dest.userId, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlocked, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }

}
