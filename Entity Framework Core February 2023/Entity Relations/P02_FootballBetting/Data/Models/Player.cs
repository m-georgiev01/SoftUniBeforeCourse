namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            PlayersStatistics = new HashSet<PlayerStatistic>();
        }

        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int SqualdNumber { get; set; }
        public bool IsInjured { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public ICollection<PlayerStatistic> PlayersStatistics { get; set; }
    }
}
