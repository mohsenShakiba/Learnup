using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedGrammarDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGrammarCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsGrammarTestCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsStoryCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsVocabCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsVocabTestCompleted",
                table: "UserLesson");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserLesson",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VoiceId",
                table: "Test",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Lesson",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "VoiceId",
                table: "Test");

            migrationBuilder.AddColumn<bool>(
                name: "IsGrammarCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGrammarTestCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStoryCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVocabCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVocabTestCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Lesson",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
