using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        // TODO: Set SongId and PerfomerId as primary keys
        // in the onModelBuilding() method in the DbContext class
        [ForeignKey(nameof(Song))]
        public int SongId { get; set; }
        public Song Song { get; set; }

        [ForeignKey(nameof(Performer))]
        public int PerformerId { get; set; }
        public Performer Performer { get; set; }
    }
}
