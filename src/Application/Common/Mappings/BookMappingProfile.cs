using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Common.Mappings;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.tagName)));

        CreateMap<BookCreateDto, Book>()
            .ConvertUsing(src => new Book(src.Title, src.Author, Guid.Empty, src.Description));

        ///< summary >
        /// All properties are ignored (Ignore) since updates are performed
        /// via dedicated methods (UpdateTitle, UpdateAuthor, etc.) in the Services 
        /// </summary>
        CreateMap<BookUpdateDto, Book>()
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CoverImageUrl, opt => opt.Ignore());

    }
}
