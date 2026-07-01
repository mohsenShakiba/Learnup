using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlacementTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlacementTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementTest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlacementResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PlacedLevel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TakenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlacementResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlacementResult_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlacementQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlacementTestId = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Skill = table.Column<int>(type: "integer", nullable: false),
                    Prompt = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacementQuestion_PlacementTest_PlacementTestId",
                        column: x => x.PlacementTestId,
                        principalTable: "PlacementTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlacementAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserPlacementResultId = table.Column<int>(type: "integer", nullable: false),
                    PlacementQuestionId = table.Column<int>(type: "integer", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlacementAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlacementAnswer_UserPlacementResult_UserPlacementResult~",
                        column: x => x.UserPlacementResultId,
                        principalTable: "UserPlacementResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlacementOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlacementQuestionId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacementOption_PlacementQuestion_PlacementQuestionId",
                        column: x => x.PlacementQuestionId,
                        principalTable: "PlacementQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlacementOption_PlacementQuestionId",
                table: "PlacementOption",
                column: "PlacementQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacementQuestion_PlacementTestId",
                table: "PlacementQuestion",
                column: "PlacementTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlacementAnswer_UserPlacementResultId",
                table: "UserPlacementAnswer",
                column: "UserPlacementResultId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlacementResult_UserId",
                table: "UserPlacementResult",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlacementOption");

            migrationBuilder.DropTable(
                name: "UserPlacementAnswer");

            migrationBuilder.DropTable(
                name: "PlacementQuestion");

            migrationBuilder.DropTable(
                name: "UserPlacementResult");

            migrationBuilder.DropTable(
                name: "PlacementTest");
        }
    }
}
