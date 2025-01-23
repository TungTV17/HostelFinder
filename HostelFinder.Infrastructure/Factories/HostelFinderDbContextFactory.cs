using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HostelFinder.Infrastructure.Factories;

public class HostelFinderDbContextFactory : IDesignTimeDbContextFactory<HostelFinderDbContext>
{
    public HostelFinderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HostelFinderDbContext>();
        return new HostelFinderDbContext(optionsBuilder.Options);
    }
}