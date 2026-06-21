using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadEntityId",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "LastReadEntityType",
                table: "UserLesson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
