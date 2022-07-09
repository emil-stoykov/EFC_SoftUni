namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportAlbumsInfo(context, 9);
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
            throw new NotImplementedException();
        }
    }
}
