using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCurrentRefToUserBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPage",
                table: "UserBook");

            migrationBuilder.AddColumn<string>(
                name: "CurrentRef",
                table: "UserBook",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRef",
                table: "UserBook");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPage",
                table: "UserBook",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
