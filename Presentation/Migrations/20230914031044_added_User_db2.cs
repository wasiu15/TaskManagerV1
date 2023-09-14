using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class added_User_db2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("16614e39-5298-48bf-b96e-92a38a7c5995"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("6861051b-5362-4ed5-8373-6d8ef0434038"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TokenGenerationTime",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "AccessToken",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { new Guid("0b676a20-256e-4b45-9247-0a836b35c67a"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals", null },
                    { new Guid("eb141467-e83f-4605-b0d3-21d55129babf"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("0b676a20-256e-4b45-9247-0a836b35c67a"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("eb141467-e83f-4605-b0d3-21d55129babf"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TokenGenerationTime",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccessToken",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { new Guid("16614e39-5298-48bf-b96e-92a38a7c5995"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals", null },
                    { new Guid("6861051b-5362-4ed5-8373-6d8ef0434038"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task", null }
                });
        }
    }
}
