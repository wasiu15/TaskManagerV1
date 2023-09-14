using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class create_notification_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("0b676a20-256e-4b45-9247-0a836b35c67a"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("eb141467-e83f-4605-b0d3-21d55129babf"));

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TaskId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ReadStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { new Guid("722dcd8d-0e84-4bb3-853c-c5dca4cb2d11"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals", null },
                    { new Guid("a3eb49a8-b7fd-4fdb-ae5d-8ad260634a29"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("722dcd8d-0e84-4bb3-853c-c5dca4cb2d11"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("a3eb49a8-b7fd-4fdb-ae5d-8ad260634a29"));

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { new Guid("0b676a20-256e-4b45-9247-0a836b35c67a"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals", null },
                    { new Guid("eb141467-e83f-4605-b0d3-21d55129babf"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task", null }
                });
        }
    }
}
