using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMotivationalSentence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MotivationalSentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sentence = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivationalSentences", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MotivationalSentences",
                columns: new[] { "Id", "IsActive", "Sentence" },
                values: new object[,]
                {
                    { 1, true, "Small steps every day become meaningful progress." },
                    { 2, true, "Stay consistent; skill grows when effort repeats." },
                    { 3, true, "One focused lesson today makes tomorrow easier." },
                    { 4, true, "Mistakes are proof that practice is happening." },
                    { 5, true, "Keep going; fluency is built one choice at a time." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MotivationalSentences");
        }
    }
}
