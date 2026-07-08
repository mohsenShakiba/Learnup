using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoryItemExpression : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoryItemExpression",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Phrase = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Meaning = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Translation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StoryItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryItemExpression", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryItemExpression_StoryItem_StoryItemId",
                        column: x => x.StoryItemId,
                        principalTable: "StoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemExpression_StoryItemId",
                table: "StoryItemExpression",
                column: "StoryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryItemExpression");
        }
    }
}
