using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedGrammarAndLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Grammar",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedTime",
                table: "Grammar",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Grammar",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Grammar",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentGrammarId",
                table: "Grammar",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GrammarLesson",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HtmlTag = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VoiceId = table.Column<int>(type: "integer", nullable: true),
                    GrammarId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarLesson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarLesson_Grammar_GrammarId",
                        column: x => x.GrammarId,
                        principalTable: "Grammar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grammar_ParentGrammarId",
                table: "Grammar",
                column: "ParentGrammarId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarLesson_GrammarId_Order",
                table: "GrammarLesson",
                columns: new[] { "GrammarId", "Order" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Grammar_Grammar_ParentGrammarId",
                table: "Grammar",
                column: "ParentGrammarId",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grammar_Grammar_ParentGrammarId",
                table: "Grammar");

            migrationBuilder.DropTable(
                name: "GrammarLesson");

            migrationBuilder.DropIndex(
                name: "IX_Grammar_ParentGrammarId",
                table: "Grammar");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Grammar");

            migrationBuilder.DropColumn(
                name: "EstimatedTime",
                table: "Grammar");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Grammar");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Grammar");

            migrationBuilder.DropColumn(
                name: "ParentGrammarId",
                table: "Grammar");
        }
    }
}
