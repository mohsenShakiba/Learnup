using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedTestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourse");

            migrationBuilder.DropTable(
                name: "UserGrammar");

            migrationBuilder.DropTable(
                name: "UserGrammarTestResult");

            migrationBuilder.DropTable(
                name: "UserStory");

            migrationBuilder.DropTable(
                name: "UserVocab");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsGrammarCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStoryCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVocabCompleted",
                table: "UserLesson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Story",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    QuestionType = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Lesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestOption_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTestResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TestId = table.Column<int>(type: "integer", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTestResult_TestOption_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "TestOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTestResult_Test_TestId",
                        column: x => x.TestId,
                        principalTable: "Test",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTestResult_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Test_LessonId",
                table: "Test",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_TestOption_TestId",
                table: "TestOption",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestResult_SelectedOptionId",
                table: "UserTestResult",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestResult_TestId",
                table: "UserTestResult",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestResult_UserId",
                table: "UserTestResult",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTestResult");

            migrationBuilder.DropTable(
                name: "TestOption");

            migrationBuilder.DropTable(
                name: "Test");

            migrationBuilder.DropColumn(
                name: "IsGrammarCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsStoryCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "IsVocabCompleted",
                table: "UserLesson");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Story");

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
                    table.ForeignKey(
                        name: "FK_GrammarTest_Grammar_GrammarId",
                        column: x => x.GrammarId,
                        principalTable: "Grammar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCourse",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    FirstVisitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourse", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_UserCourse_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourse_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGrammar",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GrammarId = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGrammar", x => new { x.UserId, x.GrammarId });
                    table.ForeignKey(
                        name: "FK_UserGrammar_Grammar_GrammarId",
                        column: x => x.GrammarId,
                        principalTable: "Grammar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGrammar_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStory",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    StoryId = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStory", x => new { x.UserId, x.StoryId });
                    table.ForeignKey(
                        name: "FK_UserStory_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserStory_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVocab",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VocabId = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVocab", x => new { x.UserId, x.VocabId });
                    table.ForeignKey(
                        name: "FK_UserVocab_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVocab_Vocab_VocabId",
                        column: x => x.VocabId,
                        principalTable: "Vocab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VocabId = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabTest_Vocab_VocabId",
                        column: x => x.VocabId,
                        principalTable: "Vocab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrammarTestOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrammarTestId = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
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
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VocabTestId = table.Column<int>(type: "integer", nullable: false)
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
                    GrammarTestId = table.Column<int>(type: "integer", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
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
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VocabTestId = table.Column<int>(type: "integer", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "IX_GrammarTest_GrammarId",
                table: "GrammarTest",
                column: "GrammarId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarTestOption_GrammarTestId",
                table: "GrammarTestOption",
                column: "GrammarTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_CourseId",
                table: "UserCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGrammar_GrammarId",
                table: "UserGrammar",
                column: "GrammarId");

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
                name: "IX_UserStory_StoryId",
                table: "UserStory",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocab_VocabId",
                table: "UserVocab",
                column: "VocabId");

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
                name: "IX_VocabTest_VocabId",
                table: "VocabTest",
                column: "VocabId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabTestOption_VocabTestId",
                table: "VocabTestOption",
                column: "VocabTestId");
        }
    }
}
