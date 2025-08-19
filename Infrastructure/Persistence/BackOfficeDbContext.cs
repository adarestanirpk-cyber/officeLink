using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class BackOfficeDbContext : DbContext
{
    public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options) : base(options)
    {
    }

    public DbSet<WFCaseLink> WFCaseLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WFCaseLink>()
            .Property(e => e.LinkType)
            .HasConversion<int>();

        modelBuilder.Entity<WFCaseLink>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<WFCaseLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SourceMainEntityName).HasMaxLength(200);
            entity.Property(e => e.TargetMainEntityName).HasMaxLength(200);
            entity.Property(e => e.SourceWFClassName).HasMaxLength(200);
            entity.Property(e => e.TargetWFClassName).HasMaxLength(200);
        });
    }
}
