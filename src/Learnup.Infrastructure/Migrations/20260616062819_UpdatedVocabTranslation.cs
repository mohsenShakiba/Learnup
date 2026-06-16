using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedVocabTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VocabTransaction");

            migrationBuilder.CreateTable(
                name: "VocabTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VocabId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Translation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Example = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ExampleTranslation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabTranslation_Vocab_VocabId",
                        column: x => x.VocabId,
                        principalTable: "Vocab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabTranslation_VocabId",
                table: "VocabTranslation",
                column: "VocabId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VocabTranslation");

            migrationBuilder.CreateTable(
                name: "VocabTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VocabId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Example = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ExampleTranslation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Translation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabTransaction_Vocab_VocabId",
                        column: x => x.VocabId,
                        principalTable: "Vocab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabTransaction_VocabId",
                table: "VocabTransaction",
                column: "VocabId");
        }
    }
}
