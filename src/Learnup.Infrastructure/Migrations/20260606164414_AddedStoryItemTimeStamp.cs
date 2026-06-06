using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedStoryItemTimeStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoryItemTimestamp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Word = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Start = table.Column<float>(type: "real", nullable: false),
                    End = table.Column<float>(type: "real", nullable: false),
                    StoryItemId = table.Column<int>(type: "integer", nullable: false),
                    StoryItemId1 = table.Column<int>(type: "integer", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_StoryItemTimestamp_StoryItem_StoryItemId1",
                        column: x => x.StoryItemId1,
                        principalTable: "StoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemTimestamp_StoryItemId",
                table: "StoryItemTimestamp",
                column: "StoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemTimestamp_StoryItemId1",
                table: "StoryItemTimestamp",
                column: "StoryItemId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryItemTimestamp");
        }
    }
}
