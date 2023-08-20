using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        public Performer()
        {
            SongPerformers = new HashSet<SongPerformer>();
        }

        public int Id { get; set; }

        [MaxLength(20)]
        public string FirstName { get; set; }

        [MaxLength(20)]
        public string LastName { get; set; }

        public int Age { get; set; }
        public decimal NetWorth { get; set; }

        public ICollection<SongPerformer> SongPerformers { get; set; }
    }
}
