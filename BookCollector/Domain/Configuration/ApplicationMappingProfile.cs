using AutoMapper;
using BookCollector.Domain.ThirdParty.Goodreads;
using BookCollector.Framework.Logging;
using BookCollector.Models;

namespace BookCollector.Domain.Configuration
{
    public class ApplicationMappingProfile : Profile
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public ApplicationMappingProfile()
        {
            log.Info("Configuring Automapper");

            CreateMap<GoodreadsCsvBook, Book>()
                .ForMember(destination => destination.Authors, opt => opt.ResolveUsing<AuthorResolver>())
                .ForMember(destination => destination.ISBN10, opt => opt.MapFrom(source => source.ISBN));
        }
    }
}
