using Microsoft.EntityFrameworkCore;
using TripMate.Helpers;
using TripMate.Models;

namespace TripMate.Data;

/// <summary>
/// Entity Framework Core database context for TripMate.
/// Uses SQL Server for data storage.
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Trip> Trips { get; set; } = null!;
    public DbSet<TripMember> TripMembers { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<ExpenseSplit> ExpenseSplits { get; set; } = null!;

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure if not already configured (e.g. from DI)
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = DatabaseConfiguration.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // TripMember - unique constraint on TripId + UserId
        modelBuilder.Entity<TripMember>(entity =>
        {
            entity.HasIndex(e => new { e.TripId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Trip).WithMany(t => t.Members).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.TripMemberships).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Expense configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasOne(e => e.Trip).WithMany(t => t.Expenses).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PaidByUser).WithMany().HasForeignKey(e => e.PaidByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        // ExpenseSplit configuration
        modelBuilder.Entity<ExpenseSplit>(entity =>
        {
            entity.HasOne(e => e.Expense).WithMany(exp => exp.Splits).HasForeignKey(e => e.ExpenseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
