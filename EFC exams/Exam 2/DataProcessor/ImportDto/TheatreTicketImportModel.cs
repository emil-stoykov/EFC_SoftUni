using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Theatre.Data.Models;

namespace Theatre.DataProcessor.ImportDto
{
    public class TheatreTicketImportModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }

        [Required]
        [JsonProperty(nameof(NumberOfHalls))]
        [Range(typeof(sbyte), "1", "10")]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [JsonProperty(nameof(Director))]
        public string Director { get; set; }

        [Required]
        [JsonProperty(nameof(Tickets))]
        public TicketImportModel[] Tickets { get; set; }
    }
}
