using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Pegasus.Entities;

namespace Pegasus.Migrations
{
    [DbContext(typeof(PegasusDbContext))]
    [Migration("20170809144305_initialCreate")]
    partial class initialCreate
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

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Pegasus.Entities.ProjectTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("Name");

                    b.Property<int>("ProjectId");

                    b.Property<string>("TaskId");

                    b.Property<int>("TaskStatusId");

                    b.Property<int>("TaskTypeId");

                    b.HasKey("Id");

                    b.ToTable("ProjectTasks");
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
        }
    }
}
