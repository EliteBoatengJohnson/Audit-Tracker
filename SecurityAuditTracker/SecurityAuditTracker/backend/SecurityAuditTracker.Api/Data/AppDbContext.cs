using Microsoft.EntityFrameworkCore;
using SecurityAuditTracker.Api.Models;

namespace SecurityAuditTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Finding> Findings => Set<Finding>();
    public DbSet<RemediationLog> RemediationLogs => Set<RemediationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Finding>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.OwnedFindings)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Finding>()
            .HasOne(f => f.CreatedBy)
            .WithMany(u => u.CreatedFindings)
            .HasForeignKey(f => f.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RemediationLog>()
            .HasOne(r => r.Finding)
            .WithMany(f => f.RemediationLogs)
            .HasForeignKey(r => r.FindingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RemediationLog>()
            .HasOne(r => r.ChangedBy)
            .WithMany(u => u.RemediationLogs)
            .HasForeignKey(r => r.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Store enums as strings for readability in Postgres (audit-friendly, easy to query/report on)
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
        modelBuilder.Entity<Finding>().Property(f => f.Severity).HasConversion<string>();
        modelBuilder.Entity<Finding>().Property(f => f.Status).HasConversion<string>();
        modelBuilder.Entity<Finding>().Property(f => f.FrameworkReference).HasConversion<string>();
        modelBuilder.Entity<RemediationLog>().Property(r => r.StatusChangedTo).HasConversion<string>();
    }
}
