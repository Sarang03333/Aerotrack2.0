using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<ServiceEvent> ServiceEvents => Set<ServiceEvent>();
    public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();
    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder b)
{
    b.Entity<Aircraft>().HasKey(x => x.AircraftId);
    b.Entity<MaintenanceTask>().HasKey(x => x.TaskId);
    b.Entity<SparePart>().HasKey(x => x.PartId);
    b.Entity<AuditLog>().HasKey(x => x.AuditId);
    b.Entity<ServiceEvent>().HasKey(x => x.Id);

    b.Entity<MaintenanceTask>()
        .HasOne(t => t.Aircraft)
        .WithMany(a => a.Tasks)
        .HasForeignKey(t => t.AircraftId)
        .OnDelete(DeleteBehavior.Cascade);

    b.Entity<AuditLog>()
        .HasOne(t => t.Aircraft)
        .WithMany(a => a.Audits)
        .HasForeignKey(t => t.AircraftId)
        .OnDelete(DeleteBehavior.Cascade);

    b.Entity<ServiceEvent>()
        .HasOne(se => se.Aircraft)
        .WithMany(a => a.ServiceHistory)
        .HasForeignKey(se => se.AircraftId)
        .OnDelete(DeleteBehavior.Cascade);

    // NEW: FK + unique TaskId (one service event per completed task)
   


b.Entity<ServiceEvent>()
 .HasOne(se => se.Task)
 .WithMany()
 .HasForeignKey(se => se.TaskId)
 .OnDelete(DeleteBehavior.NoAction);


    b.Entity<ServiceEvent>()
        .HasIndex(se => se.TaskId)
        .IsUnique()
        .HasFilter("[TaskId] IS NOT NULL"); // SQL Server null-filtered unique index
}
}
