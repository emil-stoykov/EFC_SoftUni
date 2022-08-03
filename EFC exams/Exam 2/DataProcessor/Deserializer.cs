namespace Theatre.DataProcessor
{
    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";


        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<TheatreProfile>());
            var mapper = config.CreateMapper();

            var serializer = new XmlSerializer(typeof(PlayImportModel[]), new XmlRootAttribute("Plays"));
            var playDTOs = (PlayImportModel[])serializer.Deserialize(new StringReader(xmlString));

            List<Play> validPlays = new List<Play>();

            foreach (var playDTO in playDTOs)
            {
                if (!IsValid(playDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (playDTO.Rating < 0 || playDTO.Rating > 10)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                } 

                bool isEnumValid
                    = Enum.TryParse(playDTO.Genre, out Genre genreValue);
                if (!isEnumValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan durationValue
                    = TimeSpan.ParseExact(playDTO.Duration, "c", CultureInfo.InvariantCulture);

                if (durationValue.Hours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Play playObj = new Play
                {
                    Title = playDTO.Title,
                    Duration = durationValue,
                    Rating = playDTO.Rating,
                    Genre = genreValue,
                    Description = playDTO.Description,
                    Screenwriter = playDTO.Screenwriter
                };

                sb.AppendLine(string.Format(SuccessfulImportPlay, playDTO.Title, playDTO.Genre.ToString(), playDTO.Rating));
                validPlays.Add(playObj);
            }

            context.Plays.AddRange(validPlays);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<TheatreProfile>());
            var mapper = config.CreateMapper();

            var serializer = new XmlSerializer(typeof(CastImportModel[]), new XmlRootAttribute("Casts"));
            var castDTOs = (CastImportModel[])serializer.Deserialize(new StringReader(xmlString));

            List<Cast> validCasts = new List<Cast>();

            foreach (var castDTO in castDTOs)
            {
                if (!IsValid(castDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast castObj = new Cast
                {
                    FullName = castDTO.FullName,
                    IsMainCharacter = castDTO.IsMainCharacter,
                    PhoneNumber = castDTO.PhoneNumber,
                    PlayId = castDTO.PlayId
                };

                validCasts.Add(castObj);
                sb.AppendLine(String.Format(SuccessfulImportActor, castObj.FullName, castObj.IsMainCharacter ? "main" : "lesser"));
            }

            context.Casts.AddRange(validCasts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile<TheatreProfile>());
            var mapper = config.CreateMapper();

            IEnumerable<TheatreTicketImportModel> theatreDTOs
                = JsonConvert.DeserializeObject<IEnumerable<TheatreTicketImportModel>>(jsonString);

            List<Theatre> validTheatres = new List<Theatre>();

            foreach (var theatreDTO in theatreDTOs)
            {
                if (!IsValid(theatreDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if(!theatreDTO.Tickets.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                

                Theatre theatreObj = new Theatre
                {
                    Name = theatreDTO.Name,
                    NumberOfHalls = theatreDTO.NumberOfHalls,
                    Director = theatreDTO.Director
                };

                foreach (var ticket in theatreDTO.Tickets)
                {
                    if (!IsValid(ticket))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var ticketObj = new Ticket
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber,
                        PlayId = ticket.PlayId
                    };

                    theatreObj.Tickets.Add(ticketObj);
                }

                validTheatres.Add(theatreObj);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, theatreObj.Name, theatreObj.Tickets.Count()));
            }

            context.Theatres.AddRange(validTheatres);
            context.SaveChanges(); 
            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }

    }
}
