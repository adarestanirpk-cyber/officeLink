using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

public class FrontOfficeDbContextFactory : IDesignTimeDbContextFactory<FrontOfficeDbContext>
{
    public FrontOfficeDbContext CreateDbContext(string[] args)
    {
        // تنظیم Configuration از appsettings.json پروژه FrontOfficeAPI
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FrontOfficeAPI")) // مسیر پروژه startup
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FrontOfficeDbContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("FrontOfficeConnection"));

        return new FrontOfficeDbContext(optionsBuilder.Options);
    }
}
