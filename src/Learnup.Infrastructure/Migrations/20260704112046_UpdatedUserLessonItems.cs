using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserLessonItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasGrammar",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasStory",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasTest",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasVocab",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasGrammar",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "HasStory",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "HasTest",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "HasVocab",
                table: "UserLesson");
        }
    }
}
