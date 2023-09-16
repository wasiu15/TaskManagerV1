﻿// <auto-generated />
using System;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace TaskManager.Presentation.Migrations
{
    [DbContext(typeof(RepositoryContext))]
    partial class RepositoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("ProjectUserTask", b =>
                {
                    b.Property<string>("ProjectsId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserTasksId")
                        .HasColumnType("TEXT");

                    b.HasKey("ProjectsId", "UserTasksId");

                    b.HasIndex("UserTasksId");

                    b.ToTable("ProjectUserTask");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.Notification", b =>
                {
                    b.Property<string>("NotificationId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ReadStatus")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RecievedUserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TaskId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("NotificationId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.Project", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.ProjectUserTask", b =>
                {
                    b.Property<string>("ProjectId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserTaskId")
                        .HasColumnType("TEXT");

                    b.HasKey("ProjectId", "UserTaskId");

                    b.HasIndex("UserTaskId");

                    b.ToTable("ProjectUserTasks");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TokenGenerationTime")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.UserTask", b =>
                {
                    b.Property<string>("Id")
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

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTasks");
                });

            modelBuilder.Entity("ProjectUserTask", b =>
                {
                    b.HasOne("TaskManager.Domain.Models.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskManager.Domain.Models.UserTask", null)
                        .WithMany()
                        .HasForeignKey("UserTasksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TaskManager.Domain.Models.ProjectUserTask", b =>
                {
                    b.HasOne("TaskManager.Domain.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskManager.Domain.Models.UserTask", "UserTask")
                        .WithMany()
                        .HasForeignKey("UserTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("UserTask");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.UserTask", b =>
                {
                    b.HasOne("TaskManager.Domain.Models.User", null)
                        .WithMany("UserTasks")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TaskManager.Domain.Models.User", b =>
                {
                    b.Navigation("UserTasks");
                });
#pragma warning restore 612, 618
        }
    }
}
