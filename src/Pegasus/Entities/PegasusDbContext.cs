using Microsoft.EntityFrameworkCore;

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
        public DbSet<ProjectTaskIndexer> TaskIndexers { get; set; }

        // The following is required for migrations
        public PegasusDbContext() /* Required for migrations */{ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
      
            if (optionsBuilder.IsConfigured) return;

            // sdv - The following commented out code will allow the Package Mgr Console to 
            // be debugged during update-database. Need to do that to figure out how to get 
            // the connedtion string from appsettings rather than hard-coded.
            // Note must have some actual db changes in order to trigger this.
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{

            //    System.Diagnostics.Debugger.Launch();

            //}
            //Thread.Sleep(5000);
            //Called by parameterless ctor Usually Migrations
            //var environmentName = Environment.GetEnvironmentVariable("EnvironmentName") ?? "local";

            //optionsBuilder.UseSqlServer(
            //    new ConfigurationBuilder()
            //        .SetBasePath(Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location))
            //        .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            //        .Build()
            //        .GetConnectionString("Pegasus")
            //);
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB2;Database=Pegasus;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
