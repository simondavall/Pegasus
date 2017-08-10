using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pegasus.Migrations
{
    public partial class updateDatabase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<string>(
                name: "TaskRef",
                table: "ProjectTasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskRef",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<string>(
                name: "TaskId",
                table: "ProjectTasks",
                nullable: true);
        }
    }
}
