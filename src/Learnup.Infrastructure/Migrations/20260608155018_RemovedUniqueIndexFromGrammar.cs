using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniqueIndexFromGrammar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson",
                columns: new[] { "GrammarId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson",
                columns: new[] { "GrammarId", "Order" },
                unique: true);
        }
    }
}
