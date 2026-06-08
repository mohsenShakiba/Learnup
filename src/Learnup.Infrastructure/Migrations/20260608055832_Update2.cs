using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Course_CourseId1",
                table: "Lesson");

            migrationBuilder.DropIndex(
                name: "IX_Lesson_CourseId1",
                table: "Lesson");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Lesson");

            migrationBuilder.AddColumn<int>(
                name: "LessonId1",
                table: "UserLesson",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLesson_LessonId1",
                table: "UserLesson",
                column: "LessonId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLesson_Lesson_LessonId1",
                table: "UserLesson",
                column: "LessonId1",
                principalTable: "Lesson",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLesson_Lesson_LessonId1",
                table: "UserLesson");

            migrationBuilder.DropIndex(
                name: "IX_UserLesson_LessonId1",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "LessonId1",
                table: "UserLesson");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Lesson",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId1",
                table: "Lesson",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Course_CourseId1",
                table: "Lesson",
                column: "CourseId1",
                principalTable: "Course",
                principalColumn: "Id");
        }
    }
}
