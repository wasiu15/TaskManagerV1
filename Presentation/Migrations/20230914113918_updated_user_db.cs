using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Presentation.Migrations
{
    /// <inheritdoc />
    public partial class updated_user_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("722dcd8d-0e84-4bb3-853c-c5dca4cb2d11"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("a3eb49a8-b7fd-4fdb-ae5d-8ad260634a29"));

            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { new Guid("7c174ad0-ce3b-4da6-8454-1909767c8118"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals", null },
                    { new Guid("ee72dc93-7b8a-44a3-8cf5-b1fd1788605c"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("7c174ad0-ce3b-4da6-8454-1909767c8118"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "TaskId",
                keyValue: new Guid("ee72dc93-7b8a-44a3-8cf5-b1fd1788605c"));

            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "TaskId", "Description", "DueDate", "Priority", "ProjectId", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { new Guid("722dcd8d-0e84-4bb3-853c-c5dca4cb2d11"), "Gym workout", new DateOnly(2023, 11, 10), 2, null, 1, "Fitness Goals", null },
                    { new Guid("a3eb49a8-b7fd-4fdb-ae5d-8ad260634a29"), "Research industry trends", new DateOnly(2023, 10, 12), 2, null, 2, "Work Project Task", null }
                });
        }
    }
}
