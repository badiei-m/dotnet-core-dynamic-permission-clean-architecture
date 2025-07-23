using Infrastructure.DbConfig;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TEntity> Entity<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("System");
        var assembly = typeof(UserConfiguration).Assembly;
        
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
} 
