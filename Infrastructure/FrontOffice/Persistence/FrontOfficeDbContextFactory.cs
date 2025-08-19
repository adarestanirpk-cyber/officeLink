using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.FrontOffice.Persistence;

public class FrontOfficeDbContextFactory : IDesignTimeDbContextFactory<FrontOfficeDbContext>
{
    public FrontOfficeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FrontOfficeAPI"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FrontOfficeDbContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("FrontOfficeConnection"));

        return new FrontOfficeDbContext(optionsBuilder.Options);
    }
}