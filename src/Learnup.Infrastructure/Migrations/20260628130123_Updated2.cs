using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryItemTimestamp");

            migrationBuilder.RenameColumn(
                name: "CodeHash",
                table: "UserOtp",
                newName: "Code");

            migrationBuilder.AddColumn<bool>(
                name: "IsGrammarTestCompleted",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGrammarTestCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsVocabTestCompleted",
                table: "UserLesson");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "UserOtp",
                newName: "CodeHash");

            migrationBuilder.CreateTable(
                name: "StoryItemTimestamp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoryItemId = table.Column<int>(type: "integer", nullable: false),
                    End = table.Column<float>(type: "real", nullable: false),
                    Start = table.Column<float>(type: "real", nullable: false),
                    Word = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryItemTimestamp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryItemTimestamp_StoryItem_StoryItemId",
                        column: x => x.StoryItemId,
                        principalTable: "StoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemTimestamp_StoryItemId",
                table: "StoryItemTimestamp",
                column: "StoryItemId");
        }
    }
}
