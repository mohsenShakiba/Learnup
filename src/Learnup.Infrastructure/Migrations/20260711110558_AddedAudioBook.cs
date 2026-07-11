using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAudioBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationItemVoiceTiming");

            migrationBuilder.CreateTable(
                name: "AudioBook",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Author = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Level = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Year = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WordCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CoverId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VoiceId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TimingJsonId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioBook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AudioBookListItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sentence = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Translation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    AudioBookId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioBookListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioBookListItem_AudioBook_AudioBookId",
                        column: x => x.AudioBookId,
                        principalTable: "AudioBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioBookListItemExpression",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Phrase = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Meaning = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Translation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AudioBookListItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioBookListItemExpression", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioBookListItemExpression_AudioBookListItem_AudioBookList~",
                        column: x => x.AudioBookListItemId,
                        principalTable: "AudioBookListItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioBookListItem_AudioBookId",
                table: "AudioBookListItem",
                column: "AudioBookId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioBookListItemExpression_AudioBookListItemId",
                table: "AudioBookListItemExpression",
                column: "AudioBookListItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioBookListItemExpression");

            migrationBuilder.DropTable(
                name: "AudioBookListItem");

            migrationBuilder.DropTable(
                name: "AudioBook");

            migrationBuilder.CreateTable(
                name: "ConversationItemVoiceTiming",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConversationItemId = table.Column<int>(type: "integer", nullable: false),
                    EndSeconds = table.Column<double>(type: "double precision", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    StartSeconds = table.Column<double>(type: "double precision", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationItemVoiceTiming", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationItemVoiceTiming_ConversationItem_ConversationIt~",
                        column: x => x.ConversationItemId,
                        principalTable: "ConversationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationItemVoiceTiming_ConversationItemId",
                table: "ConversationItemVoiceTiming",
                column: "ConversationItemId");
        }
    }
}
