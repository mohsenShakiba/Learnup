using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonGrammar_Grammar_GrammarId",
                table: "LessonGrammar");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonStory_Story_StoryId",
                table: "LessonStory");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonVocab_Vocab_VocabId",
                table: "LessonVocab");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Course_CourseId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGrammar_Grammar_GrammarId",
                table: "UserGrammar");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLesson_Lesson_LessonId",
                table: "UserLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStory_Story_StoryId",
                table: "UserStory");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVocab_Vocab_VocabId",
                table: "UserVocab");

            migrationBuilder.DropForeignKey(
                name: "FK_Vocab_Language_LanguageId",
                table: "Vocab");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonGrammar_Grammar_GrammarId",
                table: "LessonGrammar",
                column: "GrammarId",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonStory_Story_StoryId",
                table: "LessonStory",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonVocab_Vocab_VocabId",
                table: "LessonVocab",
                column: "VocabId",
                principalTable: "Vocab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Course_CourseId",
                table: "UserCourse",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGrammar_Grammar_GrammarId",
                table: "UserGrammar",
                column: "GrammarId",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLesson_Lesson_LessonId",
                table: "UserLesson",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStory_Story_StoryId",
                table: "UserStory",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVocab_Vocab_VocabId",
                table: "UserVocab",
                column: "VocabId",
                principalTable: "Vocab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vocab_Language_LanguageId",
                table: "Vocab",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonGrammar_Grammar_GrammarId",
                table: "LessonGrammar");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonStory_Story_StoryId",
                table: "LessonStory");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonVocab_Vocab_VocabId",
                table: "LessonVocab");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Course_CourseId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGrammar_Grammar_GrammarId",
                table: "UserGrammar");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLesson_Lesson_LessonId",
                table: "UserLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStory_Story_StoryId",
                table: "UserStory");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVocab_Vocab_VocabId",
                table: "UserVocab");

            migrationBuilder.DropForeignKey(
                name: "FK_Vocab_Language_LanguageId",
                table: "Vocab");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonGrammar_Grammar_GrammarId",
                table: "LessonGrammar",
                column: "GrammarId",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonStory_Story_StoryId",
                table: "LessonStory",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonVocab_Vocab_VocabId",
                table: "LessonVocab",
                column: "VocabId",
                principalTable: "Vocab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Course_CourseId",
                table: "UserCourse",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGrammar_Grammar_GrammarId",
                table: "UserGrammar",
                column: "GrammarId",
                principalTable: "Grammar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLesson_Lesson_LessonId",
                table: "UserLesson",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStory_Story_StoryId",
                table: "UserStory",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVocab_Vocab_VocabId",
                table: "UserVocab",
                column: "VocabId",
                principalTable: "Vocab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vocab_Language_LanguageId",
                table: "Vocab",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
