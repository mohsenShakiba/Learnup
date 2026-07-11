using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserTokenUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AvailableTokens",
                table: "UserTokenUsage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetAt",
                table: "UserTokenUsage",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "TotalTokensHistoryUsage",
                table: "UserTokenUsage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTokens",
                table: "UserTokenUsage");

            migrationBuilder.DropColumn(
                name: "ResetAt",
                table: "UserTokenUsage");

            migrationBuilder.DropColumn(
                name: "TotalTokensHistoryUsage",
                table: "UserTokenUsage");
        }
    }
}
