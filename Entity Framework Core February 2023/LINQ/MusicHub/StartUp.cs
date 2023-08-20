namespace MusicHub
{
    using System;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //Console.WriteLine(ExportAlbumsInfo(context, 9));
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .Include(a => a.Songs)
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    AlbumSongs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price.ToString("0.00"),
                        SongWriterName = s.Writer.Name,
                    })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.SongWriterName)
                        .ToArray(),
                    AlbumPrice = a.Price
                })
                .ToList()
                .OrderByDescending(a => a.AlbumPrice);

            StringBuilder sb = new StringBuilder();
            foreach (var album in albums) 
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");

                sb.AppendLine("-Songs:");
                for (int i = 0; i < album.AlbumSongs.Length; i++)
                {
                    sb.AppendLine($"---#{i + 1}");
                    sb.AppendLine($"---SongName: {album.AlbumSongs[i].SongName}");
                    sb.AppendLine($"---Price: {album.AlbumSongs[i].Price}");
                    sb.AppendLine($"---Writer: {album.AlbumSongs[i].SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice.ToString("0.00")}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var durationSpan = TimeSpan.FromSeconds(duration);

            var songs = context.Songs
                .Where(s => s.Duration > durationSpan)
                .Select(s => new
                {
                    s.Name,
                    Performers = s.SongPerformers
                        .Select(sp => new
                        {
                            FullName = sp.Performer.FirstName + " " + sp.Performer.LastName
                        })
                        .OrderBy(sp => sp.FullName)
                        .ToArray(), 
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c"),
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < songs.Count(); i++)
            {
                sb.AppendLine($"-Song #{i + 1}");
                sb.AppendLine($"---SongName: {songs[i].Name}");
                sb.AppendLine($"---Writer: {songs[i].WriterName}");

                if (songs[i].Performers.Count() > 0)
                {
                    for (int j = 0; j < songs[i].Performers.Count(); j++)
                    {
                        sb.AppendLine($"---Performer: {songs[i].Performers[j].FullName}");
                    }
                }

                sb.AppendLine($"---Album Producer: {songs[i].AlbumProducer}");
                sb.AppendLine($"---Duration: {songs[i].Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
