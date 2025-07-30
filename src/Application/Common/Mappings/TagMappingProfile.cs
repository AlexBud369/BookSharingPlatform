using AutoMapper;
using Domain.Entities;
using Application.DTOs.Tag;

namespace Application.Common.Mappings;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TagId))
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.TagName))
            .ForMember(dest => dest.BookIds, opt => opt.MapFrom(src => src.Books.Select(b => b.Id)));

        CreateMap<TagCreateDto, Tag>()
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.TagName))
            .ForMember(dest => dest.TagId, opt => opt.Ignore());
    }
}