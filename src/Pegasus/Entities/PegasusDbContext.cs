using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Entities
{
    public class PegasusDbContext : DbContext
    {
        public PegasusDbContext(DbContextOptions options) : base(options)
        {}

        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskStatusHistory> StatusHistory { get; set; }
        public DbSet<TaskStatus> TaskStatus { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }

        // The following is required for migrations
        public PegasusDbContext() /* Required for migrations */{ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            //Called by parameterless ctor Usually Migrations
            var environmentName = Environment.GetEnvironmentVariable("EnvironmentName") ?? "local";

            //optionsBuilder.UseSqlServer(
            //    new ConfigurationBuilder()
            //        .SetBasePath(Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location))
            //        .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            //        .Build()
            //        .GetConnectionString("Pegasus")
            //);
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Pegasus;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
