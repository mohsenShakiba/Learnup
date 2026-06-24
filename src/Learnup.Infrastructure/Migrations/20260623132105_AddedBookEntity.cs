using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBookEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBook_UserId",
                table: "UserBook");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "UserBook");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "UserBook");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "UserBook");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "UserBook",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<int>(
                name: "EbookId",
                table: "UserBook",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ebook",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Author = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CoverId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ebook", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBook_EbookId",
                table: "UserBook",
                column: "EbookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook_UserId_EbookId",
                table: "UserBook",
                columns: new[] { "UserId", "EbookId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBook_Ebook_EbookId",
                table: "UserBook",
                column: "EbookId",
                principalTable: "Ebook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBook_Ebook_EbookId",
                table: "UserBook");

            migrationBuilder.DropTable(
                name: "Ebook");

            migrationBuilder.DropIndex(
                name: "IX_UserBook_EbookId",
                table: "UserBook");

            migrationBuilder.DropIndex(
                name: "IX_UserBook_UserId_EbookId",
                table: "UserBook");

            migrationBuilder.DropColumn(
                name: "EbookId",
                table: "UserBook");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserBook",
                newName: "UploadedAt");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "UserBook",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "UserBook",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "UserBook",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook_UserId",
                table: "UserBook",
                column: "UserId");
        }
    }
}
