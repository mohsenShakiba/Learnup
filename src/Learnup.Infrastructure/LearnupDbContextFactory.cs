using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Learnup.Infrastructure;

public class LearnupDbContextFactory : IDesignTimeDbContextFactory<LearnupDbContext>
{
    public LearnupDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LearnupDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=learnup;Username=postgres;Password=postgres");
        return new LearnupDbContext(optionsBuilder.Options);
    }
}
