using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class PrisonerImportModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [JsonProperty(nameof(FullName))]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"(The\s)[A-Z]{1}[a-z]*")]
        [JsonProperty(nameof(Nickname))]
        public string Nickname { get; set; }

        [Required]
        [Range(18, 65)]
        [JsonProperty(nameof(Age))]
        public int Age { get; set; }

        [Required]
        [JsonProperty(nameof(IncarcerationDate))]
        public string IncarcerationDate { get; set; }

        [JsonProperty(nameof(ReleaseDate))]
        public string ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [JsonProperty(nameof(Bail))]
        public decimal? Bail { get; set; }

        [JsonProperty(nameof(CellId))]
        public int? CellId { get; set; }

        [Required]
        [JsonProperty(nameof(Mails))]
        public MailsImportModel[] Mails { get; set; }
    }
}
