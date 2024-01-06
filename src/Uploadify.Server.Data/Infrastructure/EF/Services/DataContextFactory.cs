using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Uploadify.Server.Data.Infrastructure.EF.Services;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        return new DataContext(new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql()
            .UseOpenIddict().Options);
    }
}
