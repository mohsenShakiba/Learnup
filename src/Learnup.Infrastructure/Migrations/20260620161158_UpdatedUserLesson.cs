using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastReadEntityId",
                table: "UserLesson",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastReadEntityType",
                table: "UserLesson",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitedAt",
                table: "UserLesson",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadEntityId",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "LastReadEntityType",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "LastVisitedAt",
                table: "UserLesson");
        }
    }
}
