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
    // 1. Add the Users Table
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        // Primary Keys
        b.Entity<Aircraft>().HasKey(x => x.AircraftId);
        b.Entity<MaintenanceTask>().HasKey(x => x.TaskId);
        b.Entity<SparePart>().HasKey(x => x.PartId);
        b.Entity<AuditLog>().HasKey(x => x.AuditId);
        b.Entity<ServiceEvent>().HasKey(x => x.Id);
        
        // 2. Configure User Entity
        b.Entity<User>().HasKey(x => x.UserId);
        
        // Convert string[] Roles to a single string for SQL Storage
        // e.g. ["Admin", "Manager"]  ->  "Admin,Manager"
        b.Entity<User>()
            .Property(u => u.Roles)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        // RELATIONSHIPS
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

        // ServiceEvent -> MaintenanceTask (Optional, One-to-One mostly)
        b.Entity<ServiceEvent>()
            .HasOne(se => se.Task)
            .WithMany()
            .HasForeignKey(se => se.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        // Unique index for TaskId in ServiceEvents (Null filtered)
        b.Entity<ServiceEvent>()
            .HasIndex(se => se.TaskId)
            .IsUnique()
            .HasFilter("[TaskId] IS NOT NULL");

        // 3. DATA SEEDING (Initial Users)
   
        b.Entity<User>().HasData(
            new User 
            { 
                UserId = "1", 
                Username = "admin", 
                Password = "P@ssw0rd!", 
                Roles = new[] { "Admin" } 
            },
            new User 
            { 
                UserId = "2", 
                Username = "maint", 
                Password = "P@ssw0rd!", 
                Roles = new[] { "Maintenance" } 
            },
            new User 
            { 
                UserId = "3", 
                Username = "inv", 
                Password = "P@ssw0rd!", 
                Roles = new[] { "InventoryManager" } 
            },
            new User 
            { 
                UserId = "4", 
                Username = "comp", 
                Password = "P@ssw0rd!", 
                Roles = new[] { "ComplianceOfficer" } 
            }
        );
    }
}