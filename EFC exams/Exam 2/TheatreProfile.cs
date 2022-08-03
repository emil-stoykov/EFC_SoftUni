namespace Theatre
{
    using AutoMapper;
    using Theatre.Data.Models;
    using Theatre.DataProcessor.ImportDto;

    class TheatreProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public TheatreProfile()
        {
            CreateMap<PlayImportModel, Play>();
            CreateMap<CastImportModel, Cast>();
            CreateMap<TheatreTicketImportModel, Theatre>();
        }
    }
}
