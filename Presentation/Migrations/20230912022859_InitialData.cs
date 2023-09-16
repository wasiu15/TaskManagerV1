using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Presentation.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("500a9ee9-f96b-49c6-8d07-27b2e0866ba1"), "Research industry trends", new DateTime(2023, 9, 12, 3, 28, 59, 210, DateTimeKind.Local).AddTicks(7194), 1, 1, "Work Project Task" },
                    { new Guid("77966e75-603a-43fd-91c6-009dc3e75690"), "Gym workout", new DateTime(2023, 9, 12, 3, 28, 59, 210, DateTimeKind.Local).AddTicks(7181), 0, 0, "Fitness Goals" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("500a9ee9-f96b-49c6-8d07-27b2e0866ba1"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("77966e75-603a-43fd-91c6-009dc3e75690"));
        }
    }
}
