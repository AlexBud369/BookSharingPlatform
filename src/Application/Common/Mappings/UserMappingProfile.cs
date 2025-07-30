using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs.User;
using System.Runtime.Serialization;

namespace Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom((src, dest, _, context) =>
                context.Items.TryGetValue("Role", out var role) ? role.ToString() : UserRole.User.ToString()))
            .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => src.IsBlocked))
            .ForMember(dest => dest.BookIds, opt => opt.MapFrom(src => src.Books.Select(b => b.Id)));

        CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlocked, opt => opt.Ignore())
            .ForMember(dest => dest.Books, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
    }
}