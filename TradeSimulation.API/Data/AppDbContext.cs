using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradeSimulation.API.Models;

namespace TradeSimulation.API.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<Holding> Holdings => Set<Holding>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wallet>(e =>
        {
            e.Property(w => w.Balance).HasPrecision(18, 2);
            e.HasIndex(w => w.UserId).IsUnique();
            e.HasOne(w => w.User).WithMany().HasForeignKey(w => w.UserId);
        });

        modelBuilder.Entity<Trade>(e =>
        {
            e.Property(t => t.PricePerShare).HasPrecision(18, 2);
            e.Property(t => t.TotalAmount).HasPrecision(18, 2);
            e.HasIndex(t => t.Symbol);
            e.HasIndex(t => t.UserId);
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
        });

        modelBuilder.Entity<Holding>(e =>
        {
            e.Property(h => h.AvgBuyPrice).HasPrecision(18, 2);
            e.HasIndex(h => new { h.UserId, h.Symbol }).IsUnique();
            e.HasOne(h => h.User).WithMany().HasForeignKey(h => h.UserId);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.HasIndex(t => t.UserId);
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
        });
    }
}
