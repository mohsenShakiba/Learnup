using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeitnerBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeitnerBox",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeitnerBox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeitnerBox_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeitnerBoxItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeitnerBoxId = table.Column<int>(type: "integer", nullable: false),
                    VocabId = table.Column<int>(type: "integer", nullable: false),
                    BoxLevel = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeitnerBoxItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeitnerBoxItem_LeitnerBox_LeitnerBoxId",
                        column: x => x.LeitnerBoxId,
                        principalTable: "LeitnerBox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeitnerBoxItem_Vocab_VocabId",
                        column: x => x.VocabId,
                        principalTable: "Vocab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeitnerBox_UserId",
                table: "LeitnerBox",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeitnerBoxItem_LeitnerBoxId",
                table: "LeitnerBoxItem",
                column: "LeitnerBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_LeitnerBoxItem_VocabId",
                table: "LeitnerBoxItem",
                column: "VocabId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeitnerBoxItem");

            migrationBuilder.DropTable(
                name: "LeitnerBox");
        }
    }
}
