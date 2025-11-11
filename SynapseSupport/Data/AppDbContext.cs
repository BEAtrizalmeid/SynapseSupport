using Microsoft.EntityFrameworkCore;
using SynapseSupport.Models;

namespace SynapseSupport.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Solution> Solutions => Set<Solution>();
    public DbSet<AILog> AILogs => Set<AILog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índices únicos
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Nome)
            .IsUnique();
    }
}
