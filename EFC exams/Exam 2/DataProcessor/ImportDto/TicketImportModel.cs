using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Theatre.DataProcessor.ImportDto
{
    public class TicketImportModel
    {
        [Required]
        [JsonProperty(nameof(Price))]
        [Range(typeof(decimal), "1", "100")]
        // do the check in the method :)
        public decimal Price { get; set; }

        [Required]
        [JsonProperty(nameof(RowNumber))]
        // do the check in the method :)
        [Range(typeof(sbyte), "1", "10")]
        public sbyte RowNumber { get; set; }

        [Required]
        [JsonProperty(nameof(PlayId))]
        public int PlayId { get; set; }
    }
}
