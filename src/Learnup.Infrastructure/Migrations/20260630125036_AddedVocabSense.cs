using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVocabSense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Example",
                table: "Vocab");

            migrationBuilder.DropColumn(
                name: "ExampleTranslation",
                table: "Vocab");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Vocab");

            migrationBuilder.CreateTable(
                name: "VocabSense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Translation = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Example = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ExampleTranslation = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    VocabId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabSense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabSense_Vocab_VocabId",
                        column: x => x.VocabId,
                        principalTable: "Vocab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabSense_VocabId",
                table: "VocabSense",
                column: "VocabId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VocabSense");

            migrationBuilder.AddColumn<string>(
                name: "Example",
                table: "Vocab",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExampleTranslation",
                table: "Vocab",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Vocab",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
