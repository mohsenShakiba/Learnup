using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLeitnerBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BoxLevel",
                table: "LeitnerBoxItem",
                newName: "BoxLevelId");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextReviewAt",
                table: "LeitnerBoxItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "LeitnerBoxItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoxLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    LeitnerBoxId = table.Column<int>(type: "integer", nullable: false),
                    WillReviewedIn = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoxLevel_LeitnerBox_LeitnerBoxId",
                        column: x => x.LeitnerBoxId,
                        principalTable: "LeitnerBox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeitnerBoxItem_BoxLevelId",
                table: "LeitnerBoxItem",
                column: "BoxLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxLevel_LeitnerBoxId",
                table: "BoxLevel",
                column: "LeitnerBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeitnerBoxItem_BoxLevel_BoxLevelId",
                table: "LeitnerBoxItem",
                column: "BoxLevelId",
                principalTable: "BoxLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeitnerBoxItem_BoxLevel_BoxLevelId",
                table: "LeitnerBoxItem");

            migrationBuilder.DropTable(
                name: "BoxLevel");

            migrationBuilder.DropIndex(
                name: "IX_LeitnerBoxItem_BoxLevelId",
                table: "LeitnerBoxItem");

            migrationBuilder.DropColumn(
                name: "NextReviewAt",
                table: "LeitnerBoxItem");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "LeitnerBoxItem");

            migrationBuilder.RenameColumn(
                name: "BoxLevelId",
                table: "LeitnerBoxItem",
                newName: "BoxLevel");
        }
    }
}
