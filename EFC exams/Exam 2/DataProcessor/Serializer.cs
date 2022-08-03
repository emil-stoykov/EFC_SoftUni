namespace Theatre.DataProcessor
{
    using System;
    using System.Linq;
    using Theatre.Data;
    using Newtonsoft.Json;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.DataProcessor.ExportDto;
    using System.Collections.Generic;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres
                = context.Theatres
                .ToList()
                .Where(x => x.NumberOfHalls >= numbersOfHalls && x.Tickets.Count() >= 20)
                .OrderByDescending(x => x.NumberOfHalls)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    Name = x.Name,
                    Halls = x.NumberOfHalls,
                    TotalIncome = x.Tickets.Where(x => x.RowNumber >= 1 && x.RowNumber <= 5).Sum(x => x.Price),
                    Tickets
                        = x.Tickets.Where(x => x.RowNumber >= 1 && x.RowNumber <= 5)
                        .Select(t => new
                        {
                            Price = decimal.Parse(t.Price.ToString("f2")),
                            RowNumber = t.RowNumber
                        })
                        .OrderByDescending(t => t.Price)
                        .ToArray()
                });

            return JsonConvert.SerializeObject(theatres, Formatting.Indented);  
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            //TimeSpan.ParseExact(playDTO.Duration, "c", CultureInfo.InvariantCulture);
            List<PlayExportModel> playsDTO
                = context.Plays
                .Where(x => x.Rating <= rating)
                .OrderBy(x => x.Title)
                .ThenByDescending(x => x.Genre)
                .Select(x => new PlayExportModel()
                {
                    Title = x.Title,
                    Duration = x.Duration.ToString("c"),
                    Rating = x.Rating == 0f ? "Premier" : x.Rating.ToString(CultureInfo.InvariantCulture), // 
                    Genre = x.Genre.ToString(),
                    Actors
                        = x.Casts
                        .Where(x => x.IsMainCharacter)
                        .OrderByDescending(x => x.FullName)
                        .Select(a => new ActorsExportModel
                        {
                            FullName = a.FullName,
                            MainCharacter = $"Plays main character in '{x.Title}'."
                        })
                        .ToList()
                })
                .ToList(); 

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            var serializer = new XmlSerializer(typeof(List<PlayExportModel>), new XmlRootAttribute("Plays"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            serializer.Serialize(writer, playsDTO, namespaces);

            return writer.ToString().TrimEnd();
        }
    }
}
