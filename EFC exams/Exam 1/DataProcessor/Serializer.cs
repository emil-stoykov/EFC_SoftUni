namespace SoftJail.DataProcessor
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners
                = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(y => new
                    {
                        OfficerName = y.Officer.FullName,
                        Department = y.Officer.Department.Name
                    })
                    .OrderBy(x => x.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = decimal.Parse(x.PrisonerOfficers.Sum(z => z.Officer.Salary).ToString("f2")),
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            return JsonConvert.SerializeObject(prisoners, Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] prisonerNamesArray = prisonersNames
                                         .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .ToArray();

            PrisonerExportModel[] prisonerDTOs
                = context.Prisoners
                .Where(x => prisonerNamesArray.Contains(x.FullName))
                .ProjectTo<PrisonerExportModel>(Mapper.Configuration)
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            
            var serializer
                = new XmlSerializer(typeof(PrisonerExportModel[]), new XmlRootAttribute("Prisoners"));

            serializer.Serialize(writer, prisonerDTOs, namespaces);

            return writer.ToString().TrimEnd();
        }
    }
}