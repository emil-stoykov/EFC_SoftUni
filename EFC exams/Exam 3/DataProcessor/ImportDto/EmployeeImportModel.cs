using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class EmployeeImportModel
    {
        [Required]
        [MinLength(3), MaxLength(40)]
        [RegularExpression(@"^[A-Za-z0-9]{3,}$")]
        [JsonProperty(nameof(Username))]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty(nameof(Email))]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{4}$")]
        [JsonProperty(nameof(Phone))]
        public string Phone { get; set; }

        [Required]
        [JsonProperty(nameof(Tasks))]
        public int[] Tasks { get; set; }
    }
}
