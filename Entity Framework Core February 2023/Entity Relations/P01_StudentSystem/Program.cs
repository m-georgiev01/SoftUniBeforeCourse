using P01_StudentSystem.Data;

namespace P01_StudentSystem
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            StudentSystemContext context = new StudentSystemContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}