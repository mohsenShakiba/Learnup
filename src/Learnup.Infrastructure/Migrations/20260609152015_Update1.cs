using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVisitedAt",
                table: "UserCourse");

            migrationBuilder.DropColumn(
                name: "VisitCount",
                table: "UserCourse");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Story",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Lesson",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Lesson");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitedAt",
                table: "UserCourse",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "VisitCount",
                table: "UserCourse",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
