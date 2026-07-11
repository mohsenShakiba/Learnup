using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnup.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStoryToConversationAndChat : Migration
    {
        // This migration renames two aggregates in place, preserving all data:
        //   * the AI-chat "Conversation" -> "Chat" (frees up the Conversation name), and
        //   * the "Story" aggregate -> "Conversation" (Story/StoryItem/... -> Conversation/ConversationItem/...).
        // It also drops the removed columns Story.CoverId and StoryItem.VoiceId, and renames
        // UserLesson.HasStory -> HasConversation.
        //
        // EF cannot detect renames, so the auto-scaffold produced a destructive drop/create.
        // These bodies were rewritten by hand to use rename operations instead. Ordering matters:
        // the chat "Conversation" table must be renamed to "Chat" before "Story" can take the
        // "Conversation" name.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- AI chat: Conversation -> Chat (free the "Conversation" name first) ----
            migrationBuilder.RenameColumn(name: "ConversationId", table: "ChatMessage", newName: "ChatId");
            migrationBuilder.RenameIndex(name: "IX_ChatMessage_ConversationId", table: "ChatMessage", newName: "IX_ChatMessage_ChatId");
            migrationBuilder.Sql(@"ALTER TABLE ""ChatMessage"" RENAME CONSTRAINT ""FK_ChatMessage_Conversation_ConversationId"" TO ""FK_ChatMessage_Chat_ChatId"";");

            migrationBuilder.RenameTable(name: "Conversation", newName: "Chat");
            migrationBuilder.RenameIndex(name: "IX_Conversation_UserId", table: "Chat", newName: "IX_Chat_UserId");
            migrationBuilder.Sql(@"ALTER TABLE ""Chat"" RENAME CONSTRAINT ""PK_Conversation"" TO ""PK_Chat"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Chat"" RENAME CONSTRAINT ""FK_Conversation_User_UserId"" TO ""FK_Chat_User_UserId"";");

            // ---- Story -> Conversation ----
            migrationBuilder.RenameTable(name: "Story", newName: "Conversation");
            migrationBuilder.Sql(@"ALTER TABLE ""Conversation"" RENAME CONSTRAINT ""PK_Story"" TO ""PK_Conversation"";");
            migrationBuilder.DropColumn(name: "CoverId", table: "Conversation");

            // ---- StoryItem -> ConversationItem ----
            migrationBuilder.RenameTable(name: "StoryItem", newName: "ConversationItem");
            migrationBuilder.RenameColumn(name: "StoryId", table: "ConversationItem", newName: "ConversationId");
            migrationBuilder.RenameIndex(name: "IX_StoryItem_StoryId", table: "ConversationItem", newName: "IX_ConversationItem_ConversationId");
            migrationBuilder.DropColumn(name: "VoiceId", table: "ConversationItem");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItem"" RENAME CONSTRAINT ""PK_StoryItem"" TO ""PK_ConversationItem"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItem"" RENAME CONSTRAINT ""FK_StoryItem_Story_StoryId"" TO ""FK_ConversationItem_Conversation_ConversationId"";");

            // ---- StoryItemExpression -> ConversationItemExpression ----
            migrationBuilder.RenameTable(name: "StoryItemExpression", newName: "ConversationItemExpression");
            migrationBuilder.RenameColumn(name: "StoryItemId", table: "ConversationItemExpression", newName: "ConversationItemId");
            migrationBuilder.RenameIndex(name: "IX_StoryItemExpression_StoryItemId", table: "ConversationItemExpression", newName: "IX_ConversationItemExpression_ConversationItemId");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemExpression"" RENAME CONSTRAINT ""PK_StoryItemExpression"" TO ""PK_ConversationItemExpression"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemExpression"" RENAME CONSTRAINT ""FK_StoryItemExpression_StoryItem_StoryItemId"" TO ""FK_ConversationItemExpression_ConversationItem_ConversationIte~"";");

            // ---- StoryItemVoiceTiming -> ConversationItemVoiceTiming ----
            migrationBuilder.RenameTable(name: "StoryItemVoiceTiming", newName: "ConversationItemVoiceTiming");
            migrationBuilder.RenameColumn(name: "StoryItemId", table: "ConversationItemVoiceTiming", newName: "ConversationItemId");
            migrationBuilder.RenameIndex(name: "IX_StoryItemVoiceTiming_StoryItemId", table: "ConversationItemVoiceTiming", newName: "IX_ConversationItemVoiceTiming_ConversationItemId");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemVoiceTiming"" RENAME CONSTRAINT ""PK_StoryItemVoiceTiming"" TO ""PK_ConversationItemVoiceTiming"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemVoiceTiming"" RENAME CONSTRAINT ""FK_StoryItemVoiceTiming_StoryItem_StoryItemId"" TO ""FK_ConversationItemVoiceTiming_ConversationItem_ConversationIt~"";");

            // ---- LessonStory -> LessonConversation ----
            migrationBuilder.RenameTable(name: "LessonStory", newName: "LessonConversation");
            migrationBuilder.RenameColumn(name: "StoryId", table: "LessonConversation", newName: "ConversationId");
            migrationBuilder.RenameIndex(name: "IX_LessonStory_StoryId", table: "LessonConversation", newName: "IX_LessonConversation_ConversationId");
            migrationBuilder.Sql(@"ALTER TABLE ""LessonConversation"" RENAME CONSTRAINT ""PK_LessonStory"" TO ""PK_LessonConversation"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LessonConversation"" RENAME CONSTRAINT ""FK_LessonStory_Story_StoryId"" TO ""FK_LessonConversation_Conversation_ConversationId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LessonConversation"" RENAME CONSTRAINT ""FK_LessonStory_Lesson_LessonId"" TO ""FK_LessonConversation_Lesson_LessonId"";");

            // ---- UserLesson ----
            migrationBuilder.RenameColumn(name: "HasStory", table: "UserLesson", newName: "HasConversation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ---- UserLesson ----
            migrationBuilder.RenameColumn(name: "HasConversation", table: "UserLesson", newName: "HasStory");

            // ---- LessonConversation -> LessonStory ----
            migrationBuilder.Sql(@"ALTER TABLE ""LessonConversation"" RENAME CONSTRAINT ""FK_LessonConversation_Lesson_LessonId"" TO ""FK_LessonStory_Lesson_LessonId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LessonConversation"" RENAME CONSTRAINT ""FK_LessonConversation_Conversation_ConversationId"" TO ""FK_LessonStory_Story_StoryId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""LessonConversation"" RENAME CONSTRAINT ""PK_LessonConversation"" TO ""PK_LessonStory"";");
            migrationBuilder.RenameIndex(name: "IX_LessonConversation_ConversationId", table: "LessonConversation", newName: "IX_LessonStory_StoryId");
            migrationBuilder.RenameColumn(name: "ConversationId", table: "LessonConversation", newName: "StoryId");
            migrationBuilder.RenameTable(name: "LessonConversation", newName: "LessonStory");

            // ---- ConversationItemVoiceTiming -> StoryItemVoiceTiming ----
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemVoiceTiming"" RENAME CONSTRAINT ""FK_ConversationItemVoiceTiming_ConversationItem_ConversationIt~"" TO ""FK_StoryItemVoiceTiming_StoryItem_StoryItemId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemVoiceTiming"" RENAME CONSTRAINT ""PK_ConversationItemVoiceTiming"" TO ""PK_StoryItemVoiceTiming"";");
            migrationBuilder.RenameIndex(name: "IX_ConversationItemVoiceTiming_ConversationItemId", table: "ConversationItemVoiceTiming", newName: "IX_StoryItemVoiceTiming_StoryItemId");
            migrationBuilder.RenameColumn(name: "ConversationItemId", table: "ConversationItemVoiceTiming", newName: "StoryItemId");
            migrationBuilder.RenameTable(name: "ConversationItemVoiceTiming", newName: "StoryItemVoiceTiming");

            // ---- ConversationItemExpression -> StoryItemExpression ----
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemExpression"" RENAME CONSTRAINT ""FK_ConversationItemExpression_ConversationItem_ConversationIte~"" TO ""FK_StoryItemExpression_StoryItem_StoryItemId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItemExpression"" RENAME CONSTRAINT ""PK_ConversationItemExpression"" TO ""PK_StoryItemExpression"";");
            migrationBuilder.RenameIndex(name: "IX_ConversationItemExpression_ConversationItemId", table: "ConversationItemExpression", newName: "IX_StoryItemExpression_StoryItemId");
            migrationBuilder.RenameColumn(name: "ConversationItemId", table: "ConversationItemExpression", newName: "StoryItemId");
            migrationBuilder.RenameTable(name: "ConversationItemExpression", newName: "StoryItemExpression");

            // ---- ConversationItem -> StoryItem ----
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItem"" RENAME CONSTRAINT ""FK_ConversationItem_Conversation_ConversationId"" TO ""FK_StoryItem_Story_StoryId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ConversationItem"" RENAME CONSTRAINT ""PK_ConversationItem"" TO ""PK_StoryItem"";");
            migrationBuilder.AddColumn<string>(name: "VoiceId", table: "ConversationItem", type: "character varying(255)", maxLength: 255, nullable: true);
            migrationBuilder.RenameIndex(name: "IX_ConversationItem_ConversationId", table: "ConversationItem", newName: "IX_StoryItem_StoryId");
            migrationBuilder.RenameColumn(name: "ConversationId", table: "ConversationItem", newName: "StoryId");
            migrationBuilder.RenameTable(name: "ConversationItem", newName: "StoryItem");

            // ---- Conversation -> Story (before Chat can reclaim the Conversation name) ----
            migrationBuilder.AddColumn<string>(name: "CoverId", table: "Conversation", type: "text", nullable: true);
            migrationBuilder.Sql(@"ALTER TABLE ""Conversation"" RENAME CONSTRAINT ""PK_Conversation"" TO ""PK_Story"";");
            migrationBuilder.RenameTable(name: "Conversation", newName: "Story");

            // ---- Chat -> Conversation ----
            migrationBuilder.Sql(@"ALTER TABLE ""Chat"" RENAME CONSTRAINT ""FK_Chat_User_UserId"" TO ""FK_Conversation_User_UserId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Chat"" RENAME CONSTRAINT ""PK_Chat"" TO ""PK_Conversation"";");
            migrationBuilder.RenameIndex(name: "IX_Chat_UserId", table: "Chat", newName: "IX_Conversation_UserId");
            migrationBuilder.RenameTable(name: "Chat", newName: "Conversation");

            migrationBuilder.Sql(@"ALTER TABLE ""ChatMessage"" RENAME CONSTRAINT ""FK_ChatMessage_Chat_ChatId"" TO ""FK_ChatMessage_Conversation_ConversationId"";");
            migrationBuilder.RenameIndex(name: "IX_ChatMessage_ChatId", table: "ChatMessage", newName: "IX_ChatMessage_ConversationId");
            migrationBuilder.RenameColumn(name: "ChatId", table: "ChatMessage", newName: "ConversationId");
        }
    }
}
