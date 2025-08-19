using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.BackOffice.Persistence;

public class BackOfficeOutboxDbContext : DbContext
{
    public BackOfficeOutboxDbContext(DbContextOptions<BackOfficeOutboxDbContext> options)
        : base(options) { }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
}
