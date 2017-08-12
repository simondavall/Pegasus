using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pegasus.Migrations
{
    public partial class addedTaskPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FixedInRelease",
                table: "ProjectTasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskPriorityId",
                table: "ProjectTasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedInRelease",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "TaskPriorityId",
                table: "ProjectTasks");
        }
    }
}
