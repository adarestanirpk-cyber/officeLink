using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.FrontOffice.Persistence;

public class FrontOfficeOutboxDbContext : DbContext
{
    public FrontOfficeOutboxDbContext(DbContextOptions<FrontOfficeOutboxDbContext> options)
    : base(options) { }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
}
