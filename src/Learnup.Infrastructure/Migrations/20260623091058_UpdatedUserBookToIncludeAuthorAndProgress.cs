using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserBookToIncludeAuthorAndProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "UserBook",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Progress",
                table: "UserBook",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "UserBook");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "UserBook");
        }
    }
}
