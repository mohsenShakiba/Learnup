using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedParentVocab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_Subscriptions_SubscriptionId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "ParentVocab",
                table: "Vocab",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentVocabDescription",
                table: "Vocab",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_Subscriptions_SubscriptionId",
                table: "UserSubscriptions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_Subscriptions_SubscriptionId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "ParentVocab",
                table: "Vocab");

            migrationBuilder.DropColumn(
                name: "ParentVocabDescription",
                table: "Vocab");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserSubscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_Subscriptions_SubscriptionId",
                table: "UserSubscriptions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
