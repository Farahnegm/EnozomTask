
using EnozomTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnozomTask.InfraStructure.persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }

     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Project -> Tasks
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // keep cascade here

            // Project -> TimeEntries
            modelBuilder.Entity<Project>()
                .HasMany(p => p.TimeEntries)
                .WithOne(te => te.Project)
                .HasForeignKey(te => te.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // fix

            // TaskItem -> TimeEntries
            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.TimeEntries)
                .WithOne(te => te.TaskItem)
                .HasForeignKey(te => te.TaskItemId)
                .OnDelete(DeleteBehavior.Restrict); // fix

            // User -> TimeEntries
            modelBuilder.Entity<User>()
                .HasMany(u => u.TimeEntries)
                .WithOne(te => te.User)
                .HasForeignKey(te => te.UserId)
                .OnDelete(DeleteBehavior.Restrict); // fix
        }

    }

}
