using Microsoft.EntityFrameworkCore.Migrations;
using Pegasus.Entities;

namespace Pegasus.Services
{
    public class DataMigrationSeeding : Migration
    {
        private IPegasusData _db;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _db = new SqlPegasusData(new PegasusDbContext());

            _db.AddTaskStatus(new TaskStatus { Name = "Submitted", DisplayOrder = 0 });
            _db.AddTaskStatus(new TaskStatus { Name = "In Progress", DisplayOrder = 10 });
            _db.AddTaskStatus(new TaskStatus { Name = "Completed", DisplayOrder = 99 });

            _db.AddTaskType(new TaskType { Name = "Task", DisplayOrder = 1 });
            _db.AddTaskType(new TaskType { Name = "Bug", DisplayOrder = 5 });

            _db.AddProject(new Project { Name = "TestProject", ProjectPrefix = "TST" });
            _db.AddProject(new Project { Name = "Pegasus", ProjectPrefix = "PGS"});
            _db.AddProject(new Project { Name = "Hereford Securities Ltd", ProjectPrefix = "HSL"});
            _db.AddProject(new Project { Name = "Runkeeper Analyser", ProjectPrefix = "RKA"});
            _db.AddProject(new Project { Name = "Lombardi", ProjectPrefix = "LOM"});

            _db.AddTaskIndexer(new ProjectTaskIndexer { ProjectId = 1, NextIndex = 1 });
            _db.AddTaskIndexer(new ProjectTaskIndexer { ProjectId = 2, NextIndex = 3 });
            _db.AddTaskIndexer(new ProjectTaskIndexer { ProjectId = 3, NextIndex = 14 });
            _db.AddTaskIndexer(new ProjectTaskIndexer { ProjectId = 4, NextIndex = 20 });
            _db.AddTaskIndexer(new ProjectTaskIndexer { ProjectId = 5, NextIndex = 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
