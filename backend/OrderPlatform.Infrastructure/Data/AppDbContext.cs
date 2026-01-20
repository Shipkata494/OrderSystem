using Microsoft.EntityFrameworkCore;
using OrderPlatform.Domain.Entities;

namespace OrderPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderEvent>()
            .HasIndex(e => new { e.OrderId, e.Type })
            .IsUnique();
    }
}
