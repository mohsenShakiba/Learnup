using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedGrammar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrammarId1",
                table: "GrammarLesson",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarLesson_GrammarId1",
                table: "GrammarLesson",
                column: "GrammarId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GrammarLesson_Grammar_GrammarId1",
                table: "GrammarLesson",
                column: "GrammarId1",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrammarLesson_Grammar_GrammarId1",
                table: "GrammarLesson");

            migrationBuilder.DropIndex(
                name: "IX_GrammarLesson_GrammarId1",
                table: "GrammarLesson");

            migrationBuilder.DropColumn(
                name: "GrammarId1",
                table: "GrammarLesson");
        }
    }
}
