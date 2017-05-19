using AutoMapper;
using BookCollector.Data;
using BookCollector.ThirdParty.Goodreads;

namespace BookCollector.Initialization
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GoodreadsCsvBook, Book>()
                .ForMember(destination => destination.Authors, opt => opt.ResolveUsing<AuthorResolver>())
                .ForMember(destination => destination.ISBN10, opt => opt.MapFrom(source => source.ISBN));
        }
    }
}
