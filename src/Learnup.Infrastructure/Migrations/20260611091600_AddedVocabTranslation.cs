using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVocabTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocab_Vocab_ParentVocabId",
                table: "Vocab");

            migrationBuilder.DropIndex(
                name: "IX_Vocab_ParentVocabId",
                table: "Vocab");

            migrationBuilder.DropColumn(
                name: "ParentVocabId",
                table: "Vocab");

            migrationBuilder.AddColumn<string>(
                name: "ParentVocab",
                table: "Vocab",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Vocab",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentVocab",
                table: "Vocab");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vocab");

            migrationBuilder.AddColumn<int>(
                name: "ParentVocabId",
                table: "Vocab",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vocab_ParentVocabId",
                table: "Vocab",
                column: "ParentVocabId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocab_Vocab_ParentVocabId",
                table: "Vocab",
                column: "ParentVocabId",
                principalTable: "Vocab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
