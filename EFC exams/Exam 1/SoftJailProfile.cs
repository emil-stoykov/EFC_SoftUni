namespace SoftJail
{
    using AutoMapper;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ExportDto;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Linq;

    public class SoftJailProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public SoftJailProfile()
        {
            this.CreateMap<DepartmentImportModel, Department>();
            this.CreateMap<PrisonerImportModel, Prisoner>();
            this.CreateMap<Mail, EncryptMessagesExport>()
                .ForMember(x => x.Description, mo => mo.MapFrom(s => String.Join("", s.Description.Reverse())));
            this.CreateMap<Prisoner, PrisonerExportModel>()
                .ForMember(d => d.IncarcerationDate, mo => mo.MapFrom(src => src.IncarcerationDate.ToString("yyyy-MM-dd")))
                .ForMember(d => d.EncryptedMessages, mo => mo.MapFrom(src => src.Mails));
        }
    }
}
