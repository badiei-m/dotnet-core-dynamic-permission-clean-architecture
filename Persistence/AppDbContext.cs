using Infrastructure.DbConfig;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext
{
    
    public DbSet<TEntity> Entity<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("System");
        var assembly = typeof(UserConfiguration).Assembly;
        
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
} 