using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrammarLesson_Grammar_GrammarId1",
                table: "GrammarLesson");

            migrationBuilder.DropIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson");

            migrationBuilder.DropIndex(
                name: "IX_GrammarLesson_GrammarId1",
                table: "GrammarLesson");

            migrationBuilder.DropColumn(
                name: "GrammarId1",
                table: "GrammarLesson");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarLesson_GrammarId",
                table: "GrammarLesson",
                column: "GrammarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GrammarLesson_GrammarId",
                table: "GrammarLesson");

            migrationBuilder.AddColumn<int>(
                name: "GrammarId1",
                table: "GrammarLesson",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson",
                columns: new[] { "GrammarId", "Order" });

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
    }
}
