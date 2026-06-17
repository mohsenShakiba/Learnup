using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedVocabAndGrammarTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VocabTest_VocabId",
                table: "VocabTest",
                column: "VocabId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTest_GrammarId",
                table: "GrammarTest",
                column: "GrammarId");

            migrationBuilder.AddForeignKey(
                name: "FK_GrammarTest_Grammar_GrammarId",
                table: "GrammarTest",
                column: "GrammarId",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VocabTest_Vocab_VocabId",
                table: "VocabTest",
                column: "VocabId",
                principalTable: "Vocab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrammarTest_Grammar_GrammarId",
                table: "GrammarTest");

            migrationBuilder.DropForeignKey(
                name: "FK_VocabTest_Vocab_VocabId",
                table: "VocabTest");

            migrationBuilder.DropIndex(
                name: "IX_VocabTest_VocabId",
                table: "VocabTest");

            migrationBuilder.DropIndex(
                name: "IX_GrammarTest_GrammarId",
                table: "GrammarTest");
        }
    }
}
