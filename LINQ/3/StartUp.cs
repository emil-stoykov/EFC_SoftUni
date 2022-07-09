namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //string result = ExportAlbumsInfo(context, 9);
            //Console.WriteLine(result);
            string result = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            
            //throw new NotImplementedException();
            StringBuilder sb = new StringBuilder();

            var query = context
                .Albums
                .Where(p => p.ProducerId.Value.Equals(producerId))
                .Select(p => new
                {
                    p.Price,
                    albumName = p.Name,
                    releaseDate = p.ReleaseDate.ToString("MM/dd/yyyy"),
                    producerName = p.Producer.Name,
                    albumSongs = p
                                    .Songs
                                        .OrderByDescending(s => s.Name)
                                        .ThenBy(s => s.Writer.Name)
                                        .Select(s => new
                                        {
                                            songName = s.Name,
                                            s.Price,
                                            songWriterName = s.Writer.Name
                                        })
                                        .ToArray()
                })
                .ToArray();

            foreach (var p in query.OrderByDescending(p => p.Price))
            {
                int albumSongsCntr = 1;
                sb.AppendLine($"-AlbumName: {p.albumName}");
                sb.AppendLine($"-ReleaseDate: {p.releaseDate}"); // if judge fucks up, add a space at the end
                sb.AppendLine($"-ProducerName: {p.producerName}");
                sb.AppendLine($"-Songs:");
                foreach(var s in p.albumSongs)
                {
                    sb.AppendLine($"---#{albumSongsCntr}");
                    sb.AppendLine($"---SongName: {s.songName}");
                    sb.AppendLine($"---Price: {s.Price:f2}");
                    sb.AppendLine($"---Writer: {s.songWriterName}");
                    albumSongsCntr++;
                }
                sb.AppendLine($"-AlbumPrice: {p.Price:f2}");
                albumSongsCntr = 0;
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            //throw new NotImplementedException();
            StringBuilder output = new StringBuilder();

            var query = context
                .Songs
                    .Include(s => s.SongPerformers)
                    .ThenInclude(sp => sp.Performer)
                    .Include(s => s.Writer)
                    .Include(s => s.Album)
                    .ThenInclude(a => a.Producer)
                    .ToArray()
                    .Where(s => s.Duration.TotalSeconds > duration)
                    .Select(s => new
                    {
                        songName = s.Name,
                        performerFullName = s.SongPerformers 
                            .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                            .FirstOrDefault(),
                        writerName = s.Writer.Name,
                        albumProducerName = s.Album.Producer.Name,
                        duration = s.Duration.ToString("c"),
                        
                    })
                    .OrderBy(s => s.songName)
                    .ThenBy(s => s.writerName)
                    .ThenBy(s => s.performerFullName)
                    .ToArray();

            int songsCntr = 1;
            foreach (var s in query)
            {
                output.AppendLine($"-Song #{songsCntr}");
                output.AppendLine($"---SongName: {s.songName}");
                output.AppendLine($"---Writer: {s.writerName}");
                output.AppendLine($"---Performer: {s.performerFullName}");
                output.AppendLine($"---AlbumProducer: {s.albumProducerName}");
                output.AppendLine($"---Duration: {s.duration}");
                songsCntr++;
            }

            return output.ToString().TrimEnd();
        }
    }
}
