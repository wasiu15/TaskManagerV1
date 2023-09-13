using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class Created_project_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("35f77619-97c7-48d5-a420-239677d88289"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("b9bee19b-c112-4f93-95ad-47b5de317292"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Tasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("e66fe9bc-1050-4923-9778-18a917182905"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task" },
                    { new Guid("ff45c6e4-6d01-4493-94df-a57fd09c4dcb"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks");

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("e66fe9bc-1050-4923-9778-18a917182905"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("ff45c6e4-6d01-4493-94df-a57fd09c4dcb"));

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Tasks");

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("35f77619-97c7-48d5-a420-239677d88289"), "Research industry trends", new DateOnly(2023, 10, 12), 2, 2, "Work Project Task" },
                    { new Guid("b9bee19b-c112-4f93-95ad-47b5de317292"), "Gym workout", new DateOnly(2023, 11, 10), 2, 1, "Fitness Goals" }
                });
        }
    }
}
