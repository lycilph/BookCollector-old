using AutoMapper;
using BookCollector.Data;
using BookCollector.Framework.Logging;
using BookCollector.ThirdParty.Goodreads;

namespace BookCollector.Initialization
{
    public class MappingProfile : Profile
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public MappingProfile()
        {
            log.Info("Configuring Automapper");

            CreateMap<GoodreadsCsvBook, Book>()
                .ForMember(destination => destination.Authors, opt => opt.ResolveUsing<AuthorResolver>())
                .ForMember(destination => destination.ISBN10, opt => opt.MapFrom(source => source.ISBN));

            CreateMap<Settings, Settings>();
            CreateMap<CollectionDescription, CollectionDescription>();
        }
    }
}
