using AutoMapper;
using Domain.Entities;
using Application.DTOs.Tag;

namespace Application.Common.Mappings;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.tagId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.tagName));

        CreateMap<TagCreateDto, Tag>()
            .ConvertUsing(src => new Tag(Guid.NewGuid(), src.Name));
    }
}
