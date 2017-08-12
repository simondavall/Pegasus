using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Pegasus.Entities;

namespace Pegasus.Migrations
{
    [DbContext(typeof(PegasusDbContext))]
    [Migration("20170812174952_addedTaskPriority2")]
    partial class addedTaskPriority2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Pegasus.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("ProjectPrefix");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Pegasus.Entities.ProjectTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("FixedInRelease");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("Name");

                    b.Property<int>("ProjectId");

                    b.Property<int>("TaskPriorityId");

                    b.Property<string>("TaskRef");

                    b.Property<int>("TaskStatusId");

                    b.Property<int>("TaskTypeId");

                    b.HasKey("Id");

                    b.ToTable("ProjectTasks");
                });

            modelBuilder.Entity("Pegasus.Entities.ProjectTaskIndexer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NextIndex");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.ToTable("TaskIndexers");
                });

            modelBuilder.Entity("Pegasus.Entities.TaskComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<DateTime>("Created");

                    b.Property<int>("TaskId");

                    b.HasKey("Id");

                    b.ToTable("TaskComments");
                });

            modelBuilder.Entity("Pegasus.Entities.TaskPriority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DisplayOrder");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TaskPriorities");
                });

            modelBuilder.Entity("Pegasus.Entities.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DisplayOrder");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TaskStatus");
                });

            modelBuilder.Entity("Pegasus.Entities.TaskStatusHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int>("TaskId");

                    b.Property<int>("TaskStatusId");

                    b.HasKey("Id");

                    b.ToTable("StatusHistory");
                });

            modelBuilder.Entity("Pegasus.Entities.TaskType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DisplayOrder");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TaskTypes");
                });
        }
    }
}
