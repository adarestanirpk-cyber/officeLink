using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.BackOffice.Persistence;

public class BackOfficeDbContextFactory : IDesignTimeDbContextFactory<BackOfficeDbContext>
{
    public BackOfficeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BackOfficeAPI")) // مسیر پروژه startup
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BackOfficeDbContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("BackOfficeConnection"));

        return new BackOfficeDbContext(optionsBuilder.Options);
    }
}
