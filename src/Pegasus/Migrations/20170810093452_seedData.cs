using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Pegasus.Entities;
using Pegasus.Services;

namespace Pegasus.Migrations
{
    public partial class seedData : Migration
    {
        private IPegasusData _db;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _db = new SqlPegasusData(new PegasusDbContext());

            _db.AddTaskStatus(new TaskStatus { Name = "Submitted", DisplayOrder = 0 });
            _db.AddTaskStatus(new TaskStatus { Name = "In Progress", DisplayOrder = 10 });
            _db.AddTaskStatus(new TaskStatus { Name = "Submitted", DisplayOrder = 99 });

            _db.AddTaskType(new TaskType { Name = "Bug", DisplayOrder = 0 });
            _db.AddTaskType(new TaskType { Name = "Task", DisplayOrder = 1 });

            _db.AddProject(new Project { Name = "Pegasus" });
            _db.AddTask(new ProjectTask
            {
                TaskRef = "P1-001",
                Name = "Add initial project framework",
                TaskTypeId = 2,
                ProjectId = 1,
                Modified = DateTime.Now,
                Created = DateTime.Now
            });
            _db.AddComment(new TaskComment
            {
                Comment = "Task initiated",
                Created = DateTime.Now,
                TaskId = 1
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
