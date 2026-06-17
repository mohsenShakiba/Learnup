using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExampleTranslation",
                table: "VocabTranslation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Example",
                table: "VocabTranslation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ParentVocab",
                table: "Vocab",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "GrammarTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrammarId = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarTest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VocabTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VocabId = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabTest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrammarTestOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrammarTestId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarTestOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarTestOption_GrammarTest_GrammarTestId",
                        column: x => x.GrammarTestId,
                        principalTable: "GrammarTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabTestOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VocabTestId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabTestOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabTestOption_VocabTest_VocabTestId",
                        column: x => x.VocabTestId,
                        principalTable: "VocabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGrammarTestResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GrammarTestId = table.Column<int>(type: "integer", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGrammarTestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGrammarTestResult_GrammarTestOption_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "GrammarTestOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGrammarTestResult_GrammarTest_GrammarTestId",
                        column: x => x.GrammarTestId,
                        principalTable: "GrammarTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGrammarTestResult_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVocabTestResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VocabTestId = table.Column<int>(type: "integer", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVocabTestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVocabTestResult_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVocabTestResult_VocabTestOption_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "VocabTestOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserVocabTestResult_VocabTest_VocabTestId",
                        column: x => x.VocabTestId,
                        principalTable: "VocabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTestOption_GrammarTestId",
                table: "GrammarTestOption",
                column: "GrammarTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrammarTestResult_GrammarTestId",
                table: "UserGrammarTestResult",
                column: "GrammarTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrammarTestResult_SelectedOptionId",
                table: "UserGrammarTestResult",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrammarTestResult_UserId",
                table: "UserGrammarTestResult",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabTestResult_SelectedOptionId",
                table: "UserVocabTestResult",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabTestResult_UserId",
                table: "UserVocabTestResult",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabTestResult_VocabTestId",
                table: "UserVocabTestResult",
                column: "VocabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabTestOption_VocabTestId",
                table: "VocabTestOption",
                column: "VocabTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGrammarTestResult");

            migrationBuilder.DropTable(
                name: "UserVocabTestResult");

            migrationBuilder.DropTable(
                name: "GrammarTestOption");

            migrationBuilder.DropTable(
                name: "VocabTestOption");

            migrationBuilder.DropTable(
                name: "GrammarTest");

            migrationBuilder.DropTable(
                name: "VocabTest");

            migrationBuilder.AlterColumn<string>(
                name: "ExampleTranslation",
                table: "VocabTranslation",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Example",
                table: "VocabTranslation",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentVocab",
                table: "Vocab",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
