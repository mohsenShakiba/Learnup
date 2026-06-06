using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStoryItemTimeStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryItemTimestamp_StoryItem_StoryItemId1",
                table: "StoryItemTimestamp");

            migrationBuilder.DropIndex(
                name: "IX_StoryItemTimestamp_StoryItemId1",
                table: "StoryItemTimestamp");

            migrationBuilder.DropColumn(
                name: "StoryItemId1",
                table: "StoryItemTimestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoryItemId1",
                table: "StoryItemTimestamp",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemTimestamp_StoryItemId1",
                table: "StoryItemTimestamp",
                column: "StoryItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryItemTimestamp_StoryItem_StoryItemId1",
                table: "StoryItemTimestamp",
                column: "StoryItemId1",
                principalTable: "StoryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
