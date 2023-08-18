using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Color
    {
        public Color()
        {
            PrimaryKitTeams = new HashSet<Team>();
            SecondaryKitTeams = new HashSet<Team>();
        }

        public int ColorId { get; set; }
        public string Name { get; set; }

        [InverseProperty(nameof(Team.PrimaryKitColor))]
        public ICollection<Team> PrimaryKitTeams { get; set; }

        [InverseProperty(nameof(Team.SecondaryKitColor))]
        public ICollection<Team> SecondaryKitTeams { get; set; }
    }
}
