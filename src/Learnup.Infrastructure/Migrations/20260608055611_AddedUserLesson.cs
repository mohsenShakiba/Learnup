using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Lesson",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLesson",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLesson", x => new { x.UserId, x.LessonId });
                    table.ForeignKey(
                        name: "FK_UserLesson_Lesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLesson_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId1",
                table: "Lesson",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserLesson_LessonId",
                table: "UserLesson",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Course_CourseId1",
                table: "Lesson",
                column: "CourseId1",
                principalTable: "Course",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Course_CourseId1",
                table: "Lesson");

            migrationBuilder.DropTable(
                name: "UserLesson");

            migrationBuilder.DropIndex(
                name: "IX_Lesson_CourseId1",
                table: "Lesson");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Lesson");
        }
    }
}
