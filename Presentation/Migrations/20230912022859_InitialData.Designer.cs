﻿// <auto-generated />
using System;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Presentation.Migrations
{
    [DbContext(typeof(RepositoryContext))]
    [Migration("20230912022859_InitialData")]
    partial class InitialData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("Domain.Models.UserTask", b =>
                {
                    b.Property<Guid>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TaskId");

                    b.ToTable("Tasks");

                    b.HasData(
                        new
                        {
                            TaskId = new Guid("77966e75-603a-43fd-91c6-009dc3e75690"),
                            Description = "Gym workout",
                            DueDate = new DateTime(2023, 9, 12, 3, 28, 59, 210, DateTimeKind.Local).AddTicks(7181),
                            Priority = 0,
                            Status = 0,
                            Title = "Fitness Goals"
                        },
                        new
                        {
                            TaskId = new Guid("500a9ee9-f96b-49c6-8d07-27b2e0866ba1"),
                            Description = "Research industry trends",
                            DueDate = new DateTime(2023, 9, 12, 3, 28, 59, 210, DateTimeKind.Local).AddTicks(7194),
                            Priority = 1,
                            Status = 1,
                            Title = "Work Project Task"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}