using P02_FootballBetting.Data;

namespace P02_FootballBetting
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            FootballBettingContext context = new FootballBettingContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}