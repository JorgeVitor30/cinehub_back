using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}