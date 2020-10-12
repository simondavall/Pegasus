using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Entities
{
    public class PegasusDbContext : DbContext
    {
        public PegasusDbContext(DbContextOptions<PegasusDbContext> options) : base(options)
        {}

        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskStatusHistory> StatusHistory { get; set; }
        public DbSet<TaskStatus> TaskStatus { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<ProjectTaskIndexer> TaskIndexers { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }

        // The following is required for migrations
        public PegasusDbContext() /* Required for migrations */{ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build()
                        .GetConnectionString("Pegasus")
                );
            }
        }
    }
}
